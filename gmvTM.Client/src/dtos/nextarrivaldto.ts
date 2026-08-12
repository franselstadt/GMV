export type NextArrivalDto = {
  stopCode: string
  runLabel: string
  plannedTime: string
  actualTime?: string | null
  status?: string | null
}
