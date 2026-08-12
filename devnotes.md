# dev notes

written up from the raw notes in [devnotes.txt](devnotes.txt) back to the [README](README.md).

## monday 2026-08-10

**10:00**
I started by first navigating to https://www.ladottransit.com/dash/downtown/ and studying route F, the properetries attributes and relationships it hold, to get a better understanding of how I would design the schema or objects within the domain.

Next I navigated to https://www.ladotbus.com/route/4445 or Route F (seems like the routecode or routeID of F is 4445). I then proceeded to open chrome dev tools and inspected the network monitoring rest calls to understand the relationship between the ui and entities used. I studied the real time passenger information rtpi route/endpoint. Note to LADOT the api is missing versioning.

**10:30**
I then proceeded to postman to further call and investigate the data structure of https://www.ladotbus.com/api/rtpi?path=routes%2F4445%2Fvehicles. I later reliazed that the rtpi endpoint was just a wrapper whereby the path was posted through to I assume gmv sync, ie routes/4445/vehicles and so on. I then studied the flow and calls structure.

**11:00**
I then stumbled upon LADOT's GTFS zip and studied the csv, as I am now envisioning a more generic solution or a GTFS viewer where you could post the zip and view it. i am still thinking of mocking vechiles on route with some type of either websocket as I see the LADOT is updating on an interval, non the less let me not digress from the task at hand.

**12:00**
I should remeber KISS to not overengineer or sway away from the objective. Let me start with the schema objects, I feel entityframework would be the simplist ORM here as its a relativelity small solution.

**13:00**
I decided on calling tghe solution gmv time and motion, for future scalability I will follow basic domain driven design principles, for now I am not going to use redis for static data or mediatr for in process messaging as it would defeat KISS and be overkill, however if I where to scale, this would be required.

**13:15**
I will structure the backend api with project layers: Domain, Contracts, Infastructure, Application. For now I will start with Domain and Contracts to map schema and run scenarios. I am thinking of running simulation for vehicles. Also maybe add board/signs to the map. Let me start by copying code from my older projects to get the wheel rolling fast.

**13:45**
stop draw would be an intresting fuctiuon note to self.. wait maybe also get seed data import from zip gtfs, i know it defeats kiss, but it would be so cool. Just maybe ask if I could do that and map the csv to my schema as it would be so cool as I am busy developing this now, might as well. ie use c# attributes to map csv to my objects.

**14:20**
copied my BaseItem / BaseCollection pattern over from an older project. persistable types inherit BaseItem (crud on the item itself, sync and async), collections do the bulk work. calc/non db stuff becomes ViewItems. saves me a day easily.

**14:55**
entities mapped from what I saw in the rtpi payloads this morning: route, stop, vehicle, trip, stopplan (the plan), stoptrip (what actually happened). trip is vehicle+route, stoptrip is where time and motion lands.

**15:30**
route shape is a google encoded polyline. the 5 bit chunk / zigzag decode is fiddly, claude assisted me with the decoder, credited it in the code. wrote tests against a known string to be sure.

**16:10**
haversine + walking distance along the shape. also datetime provider behind an interface so tests can freeze the clock. in a more complex system i would opt for epoch, made a note in the code.

**16:45**
collections + interfaces done. query logic sits in the concrete collection, exposed via extension methods, services never see EF.

**17:30**
ef core + sqlite wired. built route-f-seed.json by hand from the gtfs csv and the rtpi captures. stops in sequence with planned seconds from the previous stop, checked a few against ladots own times at 25mph, close enough.

**18:15**
domain tests green, polyline, coordinates, plus an architecture test so layers dont bleed into each other later.

**18:55**
enough for today. one thing keeps bugging me: a fixed timetable makes no sense for a time and motion demo. schedule should be seconds from simulation start. sleep on it.

## tuesday 2026-08-11

**09:12**
plan for today: the simulation math, application layer, then server. frontend if I survive.

**09:40**
trip path calculator first, walks the stop plan and gives me stops + cumulative planned seconds + distances in one view item.

**10:25**
position math. given elapsed seconds, mph and dwell per stop: where on the polyline is the bus and what phase is it in. claude assisted me in writing the math behind this, the segment interpolation especially. I remembered a lot from a previous similar project but simulating was new for me, left an honest comment about it in the code.

**11:05**
phases: traveling, approaching (lead seconds before arrival), doors open for the dwell, doors closing gets the last second for the announcement.

**11:40**
simulation service. stages the trip with its stoptrips, tracks an ActiveSimulation in memory. just note in a real production system I would of used redis here, in memory store is fine for this exercise.

**12:15**
found the runindex idea from my older project doesnt fit, that assumed fixed daily runs. ripped it out, everything keys off seconds since sim start plus sequence. much cleaner.

**12:50**
edge case, if the bus already passed your stop this loop the estimate should roll to the next loop, not go negative. claude assisted me with the wrap around method, wrote tests to pin it.

**13:30**
background service ticks every second and pushes positions. for the transport I went signalr, I know how to do this with kafka/rabbitmq or good old socketio but havent used signalr in a while other than blazor diff streaming, nice refresher. claude and google assisted with the websocket handling.

**14:10**
controllers, thin, just call services. exception middleware with an error envelope, i usually also add slack or email notification but not here.

**14:50**
swagger + api versioning from day one (see my monday note to LADOT, practicing what I preach).

**15:25**
serilog console + rolling file, request logging on.

**16:00**
prometheus, counters for sims started/stopped, active sims gauge, next arrival counter. simple implementation, /metrics mapped.

**16:40**
application tests with nsubstitute, integration tests with WebApplicationFactory on a throwaway sqlite file and a frozen clock.

**17:25**
tests green. frontend. vite + react + chakra + react-leaflet. carto light tiles look closest to the ladot map.

**18:05**
map draws the decoded shape in route color, stop markers, fitbounds helper. leaflets default image markers dont survive vite bundling so divicons with inline html instead.

**18:50**
side panel doubles as a wizard, pick start stop then live view with sliders for mph and seconds at stop.

**19:30**
signalr client wired, refs mirror state for the callback otherwise the closure sees stale values, classic. the bus moves down figueroa, very satisfying. thanks claude 

**20:10**
web speech api announcements, english then spanish like the real dash buses. dedupe by phase+stop key or it announces every tick.

**20:50**
check arrival flow with planned vs actual and on time / late / ahead status, behind schedule pops a dialog.

**21:14**
loop closes end to end. tomorrow: make it run on a vanilla vs2022 and polish.

## wednesday 2026-08-12

**09:12**
fresh clone on vanilla vs2022, doesnt run of course. global.json pinned an sdk I dont have, the esproj had a hardcoded sdk path and the wrong project guid in the sln. fixed all three with claude chasing the guids, added msbuild targets to check node and npm install on first debug build.

**09:45**
node_modules was installed on unix so the windows shims were missing, reinstalled. spaproxy pointed at https 5173, vite serves http. F5 works now, readme updated with requirements.

**10:20**
simple auth, its a small exam project so basic http auth with one username, no claims, sha256 hash in appsettings instead of plain text. swagger got an authorize button.

**10:55**
client kept 401ing anyway. turns out the vite proxy hit the http port, server redirected to https, and the browser strips the Authorization header on cross origin redirects. asked ai to confirm that behaviour then pointed the proxy straight at https. good one to remember.

**11:30**
hub url now carries the fleet code, /hubs/vehicle/{fleetCodes}, server groups you on connect so reconnects rejoin for free, dropped the subscribe/unsubscribe dance.

**11:55**
easter egg, special alert on the crypto.com arena stops, Go Los Angeles Dodgers Go, spoken after the approaching line with an animated gif popup that only shows while the special message plays.

**12:30**
moved the mph/meters math into a TravelTools class, and added a dev console window on debug that lists swagger, api, odata, prometheus and hub urls so I stop typing them.

**13:00**
restructured the client like the backend, dtos one type per file, globals for the constant objects, tools for the api client and speech. claude sped up the react side a lot here, I scaffold the object classes and dtos in typescript and it fills in the components around them.

**13:35**
ui pass. wizard step headers gone, 6142 preselected as start, arrival card restyled as a black led panel, amber glow, "DASH F started 12:56 from Flower St & Olympic Blvd" with planned/actual and status in led green/red/blue.
full regression, all tests green, client typechecks, fresh clone F5 works, reseed and odata checked in swagger.
readme rewrite. gave claude my notes to spellcheck and shape into the readme, it generated the erd schema of the items, the docker compose steps, the requirements table and a setup.bat that checks everything on a clean windows box.

