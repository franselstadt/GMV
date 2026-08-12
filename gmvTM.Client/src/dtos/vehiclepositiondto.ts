export type VehiclePositionDto = {
  simulationRunId: number
  tripId: number
  routeCode: string
  vehicleId: number
  vehicleNumber: string
  latitude: number
  longitude: number
  phase: string
  stopCode?: string | null
  stopName?: string | null
  secondsToStop?: number | null
  behindSchedule: boolean
  scheduleAlert?: string | null
  asOfUtc: string
}
