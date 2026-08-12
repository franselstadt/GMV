export type StopDto = {
  id: number
  stopCode: string
  name: string
  latitude: number
  longitude: number
  sequence: number
  specialAlert?: string | null
}
