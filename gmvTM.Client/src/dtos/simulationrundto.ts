export type SimulationRunDto = {
  id: number
  routeCode: string
  vehicleId: number
  vehicleNumber: string
  tripId: number
  status: string
  startStopCode: string
  averageMph: number
  averageDwellSeconds: number
  startedAtUtc: string
}
