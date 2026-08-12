import { useEffect, useRef, useState } from "react";
import { Link as RouterLink, useParams } from "react-router-dom";
import {
  AlertDialog,
  AlertDialogBody,
  AlertDialogContent,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogOverlay,
  Box,
  Button,
  Heading,
  HStack,
  IconButton,
  List,
  ListItem,
  Select,
  Slider,
  SliderFilledTrack,
  SliderThumb,
  SliderTrack,
  Spinner,
  Text,
  VStack,
} from "@chakra-ui/react";
import { MapContainer, Marker, Polyline, Popup, TileLayer, useMap } from "react-leaflet";
import L from "leaflet";
import * as signalR from "@microsoft/signalr";
import "leaflet/dist/leaflet.css";
import { Brand } from "./globals/brand";
import { Messages } from "./globals/messages";
import { VehiclePhases } from "./globals/vehiclephases";
import { ScheduleStatuses } from "./globals/schedulestatuses";
import {
  DefaultStartStopCode,
  DefaultAverageMph,
  DefaultAverageDwellSeconds,
  MaxAverageMph,
  MaxAverageDwellSeconds,
  MinAverageMph,
  MinAverageDwellSeconds,
} from "./globals/appconstants";
import type { NextArrivalDto, RouteDto, SimulationRunDto, StopDto, VehiclePositionDto } from "./dtos";
import { api, routePath } from "./tools/apitools";
import { announcePhase } from "./tools/speechtools";

//leaflets default image markers dont survive vite bundling, divIcons with inline html are way less hassle
const stopIcon = L.divIcon({
  className: "",
  html: `<div style="width:10px;height:10px;border-radius:50%;background:${Brand.Blue};border:2px solid #fff;box-shadow:0 0 0 1px ${Brand.Blue}"></div>`,
  iconSize: [10, 10],
  iconAnchor: [5, 5],
});

//inline svg bus so I dont need an extra asset, pink circle matches the dash branding
const busIcon = L.divIcon({
  className: "",
  html: `<div style="width:32px;height:32px;border-radius:50%;background:${Brand.DashPink};border:2px solid #fff;box-shadow:0 1px 5px rgba(0,0,0,.4);display:flex;align-items:center;justify-content:center"><svg width="20" height="20" viewBox="0 0 24 24" fill="#ffffff"><path d="M4 16c0 .88.39 1.67 1 2.22V20c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-1h8v1c0 .55.45 1 1 1h1c.55 0 1-.45 1-1v-1.78c.61-.55 1-1.34 1-2.22V6c0-3.5-3.58-4-8-4s-8 .5-8 4v10zm3.5 1c-.83 0-1.5-.67-1.5-1.5S6.67 14 7.5 14s1.5.67 1.5 1.5S8.33 17 7.5 17zm9 0c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zm1.5-6H6V6h12v5z"/></svg></div>`,
  iconSize: [32, 32],
  iconAnchor: [16, 16],
});

//react-leaflet has no declarative fitBounds, this little helper zooms to the route once the shape is in
function FitBounds({ points }: { points: [number, number][] }) {
  const map = useMap();
  useEffect(() => {
    if (points.length === 0) return;
    map.fitBounds(L.latLngBounds(points), { padding: [48, 48] });
  }, [map, points]);
  return null;
}

function formatTime(value: string): string {
  return value.length >= 5 ? value.slice(0, 5) : value;
}

function phaseLabel(phase: string | null): string {
  if (!phase) return Messages.SimIdle;
  if (phase === VehiclePhases.Approaching) return "Approaching";
  if (phase === VehiclePhases.DoorsOpen) return "Doors open";
  if (phase === VehiclePhases.DoorsClosing) return "Doors closing";
  if (phase === VehiclePhases.Completed) return "Completed";
  if (phase === VehiclePhases.Traveling) return Messages.SimRunning;
  return phase;
}

//"DASH F 12:43" -> "DASH F started 12:43 from <stop>", the time is always the last token of the run label
function runStartedLabel(runLabel: string, startStopName: string): string {
  const parts = runLabel.trim().split(" ");
  const time = parts.pop() ?? "";
  return Messages.RunStartedLine(parts.join(" "), time, startStopName);
}

function formatClock(date: Date): string {
  return date.toLocaleString(undefined, {
    weekday: "short",
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
  });
}

function scheduleStatusLabel(status: string | null | undefined): string {
  if (status === ScheduleStatuses.OnTime) return Messages.OnTime;
  if (status === ScheduleStatuses.RunningLate) return Messages.RunningLate;
  if (status === ScheduleStatuses.Ahead) return Messages.Ahead;
  return "";
}

function ledStatusClass(status: string | null | undefined): string {
  if (status === ScheduleStatuses.RunningLate) return "led-red";
  if (status === ScheduleStatuses.Ahead) return "led-blue";
  if (status === ScheduleStatuses.OnTime) return "led-green";
  return "led-amber";
}

function normalizeRouteCode(code: string | undefined): string {
  return (code ?? "").trim().toUpperCase();
}

export default function App() {
  const { routeCode: routeCodeParam } = useParams<{ routeCode: string }>();
  const routeCode = normalizeRouteCode(routeCodeParam);

  const [route, setRoute] = useState<RouteDto | null>(null);
  const [routeMissing, setRouteMissing] = useState(false);
  const [loading, setLoading] = useState(true);
  const [stops, setStops] = useState<StopDto[]>([]);
  const [startStop, setStartStop] = useState("");
  const [arrivalStop, setArrivalStop] = useState("");
  const [arrival, setArrival] = useState<NextArrivalDto | null>(null);
  const [shapePoints, setShapePoints] = useState<[number, number][]>([]);
  const [routeColor, setRouteColor] = useState<string>(Brand.DashPink);
  const [mph, setMph] = useState(DefaultAverageMph);
  const [averageDwellSeconds, setAverageDwellSeconds] = useState(DefaultAverageDwellSeconds);
  const [wizardStep, setWizardStep] = useState<"setup" | "live">("setup");
  const [clockNow, setClockNow] = useState(() => new Date());
  const [busy, setBusy] = useState(false);
  const [simBusy, setSimBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [scheduleAlert, setScheduleAlert] = useState<string | null>(null);
  const [specialAlertPopup, setSpecialAlertPopup] = useState<string | null>(null);
  const [run, setRun] = useState<SimulationRunDto | null>(null);
  const [vehicle, setVehicle] = useState<VehiclePositionDto | null>(null);
  const [statusLine, setStatusLine] = useState<string>(Messages.SimIdle);
  const [menuOpen, setMenuOpen] = useState(true);
  //the signalr callback lives outside reacts render cycle, so anything it needs to read goes
  //into refs, otherwise the closure keeps seeing the state from when the handler was registered
  const lastSpokenKey = useRef<string | null>(null);
  const lastAlertKey = useRef<string | null>(null);
  const alertCancelRef = useRef<HTMLButtonElement | null>(null);
  const runIdRef = useRef<number | null>(null);
  const routeCodeRef = useRef(routeCode);
  const stopsRef = useRef<StopDto[]>([]);
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const vehicleNumberRef = useRef<string | null>(null);

  useEffect(() => {
    runIdRef.current = run?.id ?? null;
  }, [run]);

  useEffect(() => {
    stopsRef.current = stops;
  }, [stops]);

  useEffect(() => {
    routeCodeRef.current = routeCode;
  }, [routeCode]);

  //load everything for the route in one go and reset the whole wizard when the code changes
  useEffect(() => {
    let cancelled = false;
    (async () => {
      setLoading(true);
      setRouteMissing(false);
      setError(null);
      setRoute(null);
      setStops([]);
      setShapePoints([]);
      setArrival(null);
      setStartStop("");
      setArrivalStop("");
      setVehicle(null);
      setRun(null);
      runIdRef.current = null;
      setWizardStep("setup");
      setStatusLine(Messages.SimIdle);
      setMph(DefaultAverageMph);
      setAverageDwellSeconds(DefaultAverageDwellSeconds);
      setScheduleAlert(null);
      lastAlertKey.current = null;

      try {
        const matched = await api<RouteDto>(routePath(routeCode));
        const color = matched.color ? `#${String(matched.color).replace(/^#/, "")}` : Brand.DashPink;

        const [shape, stopsPage] = await Promise.all([
          api<{ points: { latitude: number; longitude: number }[] }>(routePath(routeCode, "/shape")),
          api<{ items: StopDto[] }>(routePath(routeCode, "/stops?page=1&pageSize=200")),
        ]);

        if (cancelled) return;

        const stopItems = stopsPage.items ?? [];
        //preselect the demo start stop when the route has it, otherwise just take the first one
        const defaultStop =
          stopItems.find((s) => s.stopCode === DefaultStartStopCode)?.stopCode ?? stopItems[0]?.stopCode ?? "";

        setRoute(matched);
        setRouteColor(color);
        setShapePoints((shape.points ?? []).map((p) => [p.latitude, p.longitude]));
        setStops(stopItems);
        setStartStop(defaultStop);
        setArrivalStop(stopItems[1]?.stopCode ?? defaultStop);
        setLoading(false);
      } catch (err) {
        if (cancelled) return;
        const message = err instanceof Error ? err.message : Messages.FailedToLoad;
        if (/not found/i.test(message) || /Route '/i.test(message)) {
          setRouteMissing(true);
        } else {
          setError(message);
        }
        setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [routeCode]);

  useEffect(() => {
    const id = window.setInterval(() => setClockNow(new Date()), 1000);
    return () => window.clearInterval(id);
  }, []);

  //make sure the websocket doesnt outlive the component
  useEffect(() => () => disconnectFromVehicle(), []);

  function handlePositionUpdate(dto: VehiclePositionDto) {
    //updates from an old run can still arrive right after a restart, just drop them
    if (!runIdRef.current || dto.simulationRunId !== runIdRef.current) return;
    if (dto.routeCode && dto.routeCode.toUpperCase() !== routeCodeRef.current.toUpperCase()) {
      return;
    }

    setVehicle(dto);
    setStatusLine(phaseLabel(dto.phase));
    if (dto.tripId) {
      setRun((current) => (current && current.tripId !== dto.tripId ? { ...current, tripId: dto.tripId } : current));
    }

    if (dto.behindSchedule && dto.scheduleAlert) {
      const alertKey = `${dto.simulationRunId}:${dto.scheduleAlert}`;
      if (lastAlertKey.current !== alertKey) {
        lastAlertKey.current = alertKey;
        setScheduleAlert(dto.scheduleAlert);
      }
    }

    const speakable =
      dto.phase === VehiclePhases.Approaching || dto.phase === VehiclePhases.DoorsOpen || dto.phase === VehiclePhases.DoorsClosing;

    if (!speakable) return;

    //ticks come in every second so without this key the bus would announce the same stop over and over
    const key = `${dto.simulationRunId}:${dto.phase}:${dto.stopCode ?? ""}`;
    if (lastSpokenKey.current === key) return;
    lastSpokenKey.current = key;

    const specialAlert = dto.stopCode
      ? (stopsRef.current.find((s) => s.stopCode.toUpperCase() === dto.stopCode!.toUpperCase())?.specialAlert ?? null)
      : null;

    //the dodgers popup should only be up while the special message itself is being spoken,
    //not during the normal approaching announcement, hence the two callbacks
    void announcePhase(
      dto.phase,
      dto.stopName,
      specialAlert,
      () => setSpecialAlertPopup(specialAlert),
      () => setSpecialAlertPopup(null),
    );
  }

  function disconnectFromVehicle() {
    const connection = connectionRef.current;
    connectionRef.current = null;
    vehicleNumberRef.current = null;
    if (connection) void connection.stop();
  }

  //the fleet code sits in the hub url itself so the server groups us on connect,
  //which also means an automatic reconnect rejoins the group without any resubscribe dance
  function connectToVehicle(fleetCode: string) {
    if (vehicleNumberRef.current === fleetCode && connectionRef.current) return;
    disconnectFromVehicle();
    vehicleNumberRef.current = fleetCode;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`/hubs/vehicle/${encodeURIComponent(fleetCode)}`)
      .withAutomaticReconnect()
      .build();

    connection.on("positionUpdate", handlePositionUpdate);
    connectionRef.current = connection;
    void connection.start().catch(console.error);
  }

  async function startSimulation() {
    if (!startStop) return;
    setSimBusy(true);
    setError(null);
    lastSpokenKey.current = null;
    try {
      const created = await api<SimulationRunDto>(routePath(routeCode, "/simulations"), {
        method: "POST",
        body: JSON.stringify({
          stopCode: startStop,
          averageMph: mph || DefaultAverageMph,
          averageDwellSeconds: averageDwellSeconds || DefaultAverageDwellSeconds,
        }),
      });
      runIdRef.current = created.id;
      setRun(created);
      connectToVehicle(created.vehicleNumber);
      setMph(created.averageMph);
      setAverageDwellSeconds(created.averageDwellSeconds);
      setStatusLine(Messages.SimRunning);
      setVehicle(null);
      setScheduleAlert(null);
      lastAlertKey.current = null;
      setArrival(null);
      setArrivalStop((prev) => prev || startStop);
      setWizardStep("live");
    } catch (err) {
      setError(err instanceof Error ? err.message : Messages.FailedToLoad);
    } finally {
      setSimBusy(false);
    }
  }

  //same start stop, new slider values, the server just starts a fresh run
  async function applyLiveSettings() {
    if (!run?.startStopCode) return;
    setSimBusy(true);
    setError(null);
    lastSpokenKey.current = null;
    try {
      const created = await api<SimulationRunDto>(routePath(routeCode, "/simulations"), {
        method: "POST",
        body: JSON.stringify({
          stopCode: run.startStopCode,
          averageMph: mph || DefaultAverageMph,
          averageDwellSeconds: averageDwellSeconds || DefaultAverageDwellSeconds,
        }),
      });
      runIdRef.current = created.id;
      setRun(created);
      connectToVehicle(created.vehicleNumber);
      setStatusLine(Messages.SimRunning);
      setVehicle(null);
      setScheduleAlert(null);
      lastAlertKey.current = null;
      setArrival(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : Messages.FailedToLoad);
    } finally {
      setSimBusy(false);
    }
  }

  async function endSimulationAndBack() {
    setSimBusy(true);
    try {
      if (run) {
        await api<void>(`/simulations/${run.id}`, { method: "DELETE" });
      }
      runIdRef.current = null;
      setRun(null);
      disconnectFromVehicle();
      setVehicle(null);
      setArrival(null);
      setStatusLine(Messages.SimIdle);
      setMph(DefaultAverageMph);
      setAverageDwellSeconds(DefaultAverageDwellSeconds);
      setWizardStep("setup");
    } catch (err) {
      setError(err instanceof Error ? err.message : Messages.FailedToLoad);
    } finally {
      setSimBusy(false);
    }
  }

  async function fetchNextArrivals() {
    if (!run || !arrivalStop) return;
    setBusy(true);
    setError(null);
    try {
      const result = await api<NextArrivalDto>(routePath(routeCode, `/stops/${encodeURIComponent(arrivalStop)}/arrivals/next`));
      setArrival(result);
    } catch (err) {
      setArrival(null);
      setError(err instanceof Error ? err.message : Messages.FailedToLoad);
    } finally {
      setBusy(false);
    }
  }

  if (loading) {
    return (
      <VStack h="100vh" justify="center" bg={Brand.PageBg}>
        <Spinner color={Brand.Blue} />
        <Text color={Brand.Muted}>{Messages.Loading}</Text>
      </VStack>
    );
  }

  if (routeMissing) {
    return (
      <VStack h="100vh" justify="center" spacing={4} bg={Brand.PageBg} px={6} textAlign="center">
        <Box as="img" src="/gmv-logo.png" alt={Messages.Brand} h="72px" borderRadius="md" />
        <Heading as="h1" size="md" color={Brand.Text}>
          {Messages.RouteNotFound}
        </Heading>
        <Text color={Brand.Muted} maxW="360px">
          {Messages.RouteNotFoundHint}
        </Text>
        <Button as={RouterLink} to={`/route/${Messages.DefaultRouteCode}`} bg={Brand.Blue} color="white" _hover={{ bg: Brand.BlueHover }}>
          {Messages.BackToRouteF}
        </Button>
      </VStack>
    );
  }

  //the side panel doubles as the wizard, step 1 is setup and step 2 is the live run
  const menuBody = menuOpen && (
    <Box overflowY="auto" px={3} py={3}>
      <Box bg={routeColor} color="white" px={2.5} py={1.5} borderRadius="sm" mb={3}>
        <Text fontWeight="bold" fontSize="xs">
          DASH {route?.shortName ?? routeCode}
        </Text>
        <Text fontSize="2xs" opacity={0.92}>
          {route?.longName}
        </Text>
      </Box>

      <Text mb={0.5} fontSize="2xs" fontWeight="semibold" color={Brand.Muted} textTransform="uppercase">
        {Messages.CurrentDateTime}
      </Text>
      <Text mb={3} fontSize="xs" fontWeight="bold" color={Brand.Text}>
        {formatClock(clockNow)}
      </Text>

      {wizardStep === "setup" ? (
        <>
          <Box bg="#eaf2fc" border="1px solid" borderColor={Brand.Blue} borderRadius="md" px={2.5} py={2} mb={2.5}>
            <Text fontSize="xs" fontWeight="bold" color={Brand.Blue} mb={0.5}>
              {Messages.HowTimingWorksTitle}
            </Text>
            <Text fontSize="2xs" color={Brand.Text}>
              {Messages.HowTimingWorksBody}
            </Text>
          </Box>
          <Text mb={1} fontSize="xs" fontWeight="semibold" color={Brand.Muted}>
            {Messages.StartStop}
          </Text>
          <Select
            value={startStop}
            onChange={(e) => setStartStop(e.target.value)}
            bg="white"
            borderColor={Brand.InputBorder}
            size="xs"
            borderRadius="md"
            mb={2.5}>
            {stops.map((s) => (
              <option key={s.stopCode} value={s.stopCode}>
                {s.stopCode} — {s.name}
              </option>
            ))}
          </Select>
          <Text fontSize="2xs" color={Brand.Muted} mb={2.5}>
            {Messages.OnTimeBaseline}
          </Text>
          <Button
            w="100%"
            bg={Brand.Blue}
            color="white"
            _hover={{ bg: Brand.BlueHover }}
            onClick={() => void startSimulation()}
            isLoading={simBusy}
            isDisabled={!startStop}
            size="xs"
            py={3.5}
            mb={2.5}>
            {Messages.StartSimulation}
          </Button>
        </>
      ) : (
        <>
          {/* only the words that changed flip over, like the split flap boards at the sf ferry terminal */}
          <HStack mb={2.5} spacing={1.5} fontSize="xs" flexWrap="wrap" rowGap={0.5}>
            <Text fontWeight="bold" color={Brand.Text}>
              {run?.vehicleNumber ?? ""}
            </Text>
            <Text as="span" key={`phase-${statusLine}`} className="flip-word" fontWeight="bold" color={Brand.DashPink}>
              {statusLine}
            </Text>
            {vehicle?.stopName && (
              <Text as="span" key={`stop-${vehicle.stopName}`} className="flip-word" color={Brand.Muted}>
                {vehicle.stopName}
              </Text>
            )}
          </HStack>

          <Text mb={0.5} fontSize="xs" fontWeight="semibold" color={Brand.Muted}>
            {Messages.AverageMph}: {mph}
          </Text>
          <Slider
            aria-label={Messages.AverageMph}
            min={MinAverageMph}
            max={MaxAverageMph}
            step={1}
            value={mph}
            onChange={setMph}
            mb={3}
            size="sm"
            colorScheme="blue">
            <SliderTrack bg={Brand.Border}>
              <SliderFilledTrack bg={Brand.Blue} />
            </SliderTrack>
            <SliderThumb />
          </Slider>
          <Text mb={0.5} fontSize="xs" fontWeight="semibold" color={Brand.Muted}>
            {Messages.SecondsAtStop}: {averageDwellSeconds}
          </Text>
          <Slider
            aria-label={Messages.SecondsAtStop}
            min={MinAverageDwellSeconds}
            max={MaxAverageDwellSeconds}
            step={1}
            value={averageDwellSeconds}
            onChange={setAverageDwellSeconds}
            mb={2.5}
            size="sm"
            colorScheme="blue">
            <SliderTrack bg={Brand.Border}>
              <SliderFilledTrack bg={Brand.Blue} />
            </SliderTrack>
            <SliderThumb />
          </Slider>
          <HStack mb={3}>
            <Button
              flex="1"
              bg={Brand.Blue}
              color="white"
              _hover={{ bg: Brand.BlueHover }}
              onClick={() => void applyLiveSettings()}
              isLoading={simBusy}
              size="xs"
              py={3.5}>
              {Messages.RestartSimulation}
            </Button>
            <Button
              flex="1"
              variant="outline"
              borderColor={Brand.Border}
              onClick={() => void endSimulationAndBack()}
              isLoading={simBusy}
              size="xs"
              py={3.5}>
              {Messages.BackToSetup}
            </Button>
          </HStack>

          <Heading as="h2" size="xs" mb={1.5} color={Brand.Text}>
            {Messages.NextArrival}
          </Heading>
          <Text mb={1} fontSize="xs" fontWeight="semibold" color={Brand.Muted}>
            {Messages.ArrivalStop}
          </Text>
          <Select
            value={arrivalStop}
            onChange={(e) => setArrivalStop(e.target.value)}
            bg="white"
            borderColor={Brand.InputBorder}
            size="xs"
            borderRadius="md"
            mb={2.5}>
            {stops.map((s) => (
              <option key={s.stopCode} value={s.stopCode}>
                {s.stopCode} — {s.name}
              </option>
            ))}
          </Select>
          <Button
            w="100%"
            bg={Brand.Blue}
            color="white"
            _hover={{ bg: Brand.BlueHover }}
            onClick={() => void fetchNextArrivals()}
            isLoading={busy}
            isDisabled={!arrivalStop}
            size="xs"
            py={3.5}
            mb={2.5}>
            {Messages.NextArrival}
          </Button>
          <List spacing={2} fontSize="xs" mb={2.5}>
            {!arrival && !busy && <ListItem color={Brand.Muted}>{Messages.NoUpcomingArrivals}</ListItem>}
            {arrival && (
              <ListItem>
                <Box className="led-panel" px={3} py={2.5}>
                  <Text className="led-amber" fontSize="xs" mb={1.5} lineHeight="short">
                    {runStartedLabel(
                      arrival.runLabel,
                      run ? (stops.find((s) => s.stopCode === run.startStopCode)?.name ?? run.startStopCode) : "",
                    )}
                  </Text>
                  <HStack justify="space-between" mb={0.5}>
                    <Text className="led-amber" opacity={0.65} fontSize="2xs" textTransform="uppercase">
                      {Messages.PlannedArrival}
                    </Text>
                    <Text className="led-amber" fontSize="sm">
                      {formatTime(arrival.plannedTime)}
                    </Text>
                  </HStack>
                  <HStack justify="space-between" mb={arrival.status ? 1 : 0}>
                    <Text className="led-amber" opacity={0.65} fontSize="2xs" textTransform="uppercase">
                      {Messages.ActualArrival}
                    </Text>
                    <Text className="led-amber" fontSize="sm">
                      {arrival.actualTime ? formatTime(arrival.actualTime) : "--:--"}
                    </Text>
                  </HStack>
                  {arrival.status && (
                    <Text className={ledStatusClass(arrival.status)} fontSize="xs" textTransform="uppercase">
                      {scheduleStatusLabel(arrival.status)}
                    </Text>
                  )}
                </Box>
              </ListItem>
            )}
          </List>
        </>
      )}

      {error && (
        <Text mt={2} color="red.600" fontSize="sm">
          {error}
        </Text>
      )}
    </Box>
  );

  return (
    <Box h="100vh" w="100%" overflow="hidden" bg={Brand.PageBg} display="flex" flexDirection="column" position="relative">
      <Box flex={{ base: "1 1 52%", md: "1 1 auto" }} minH={{ base: "42vh", md: 0 }} h={{ md: "100%" }} position="relative">
        <MapContainer key={route?.id ?? routeCode} center={[34.05, -118.25]} zoom={13} style={{ height: "100%", width: "100%" }}>
          {/* carto light tiles, closest free ones to the ladot map style */}
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OSM</a> &copy; CARTO'
            url="https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png"
          />
          <FitBounds points={shapePoints} />
          {shapePoints.length > 0 && <Polyline positions={shapePoints} pathOptions={{ color: routeColor, weight: 6 }} />}
          {stops.map((s) => (
            <Marker key={s.stopCode} position={[s.latitude, s.longitude]} icon={stopIcon}>
              <Popup>
                <strong>{s.name}</strong>
                <br />
                {s.stopCode}
              </Popup>
            </Marker>
          ))}
          {vehicle && (
            <Marker position={[vehicle.latitude, vehicle.longitude]} icon={busIcon}>
              <Popup>
                {vehicle.vehicleNumber}
                <br />
                {phaseLabel(vehicle.phase)}
                {vehicle.stopName ? ` · ${vehicle.stopName}` : ""}
              </Popup>
            </Marker>
          )}
        </MapContainer>
      </Box>

      <Box
        zIndex={1000}
        position={{ base: "relative", md: "absolute" }}
        top={{ base: "auto", md: 4 }}
        left={{ base: 0, md: 4 }}
        right={{ base: 0, md: "auto" }}
        bottom={{ base: 0, md: "auto" }}
        w={{ base: "100%", md: "340px" }}
        maxH={{ base: "48vh", md: "calc(100vh - 32px)" }}
        bg={Brand.PanelBg}
        borderRadius={{ base: "16px 16px 0 0", md: "md" }}
        borderWidth="1px"
        borderColor={Brand.Border}
        boxShadow={Brand.Shadow}
        overflow="hidden"
        display="flex"
        flexDirection="column"
        flexShrink={0}>
        <HStack justify="space-between" px={4} py={3} borderBottomWidth="1px" borderColor={Brand.Border}>
          <Box as="img" src="/gmv-logo.png" alt={Messages.Brand} h="40px" borderRadius="sm" />
          <IconButton
            aria-label={menuOpen ? Messages.CloseMenu : Messages.OpenMenu}
            size="sm"
            variant="ghost"
            onClick={() => setMenuOpen((o) => !o)}
            icon={
              <Box aria-hidden w="18px">
                <Box h="2px" bg={Brand.Text} mb="4px" />
                <Box h="2px" bg={Brand.Text} mb="4px" />
                <Box h="2px" bg={Brand.Text} />
              </Box>
            }
          />
        </HStack>
        {menuBody}
      </Box>

      <AlertDialog isOpen={!!scheduleAlert} leastDestructiveRef={alertCancelRef} onClose={() => setScheduleAlert(null)}>
        <AlertDialogOverlay>
          <AlertDialogContent>
            <AlertDialogHeader fontSize="lg" fontWeight="bold">
              {Messages.ScheduleAlertTitle}
            </AlertDialogHeader>
            <AlertDialogBody>{scheduleAlert}</AlertDialogBody>
            <AlertDialogFooter>
              <Button ref={alertCancelRef} onClick={() => setScheduleAlert(null)} bg={Brand.Blue} color="white">
                {Messages.ScheduleAlertDismiss}
              </Button>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialogOverlay>
      </AlertDialog>

      {/* dodgers gif while the special announcement plays, pointerEvents none so it never blocks the map */}
      {specialAlertPopup && (
        <Box
          position="fixed"
          inset="0"
          display="flex"
          alignItems="center"
          justifyContent="center"
          bg="blackAlpha.600"
          zIndex={2000}
          pointerEvents="none">
          <Box bg={Brand.PanelBg} borderRadius="xl" boxShadow={Brand.Shadow} overflow="hidden" maxW="480px" mx={4}>
            <img src={Messages.SpecialAlertImage} alt={specialAlertPopup} style={{ display: "block", width: "100%" }} />
            <Text fontSize="xl" fontWeight="bold" textAlign="center" p={4} color={Brand.Blue}>
              {specialAlertPopup}
            </Text>
          </Box>
        </Box>
      )}
    </Box>
  );
}
