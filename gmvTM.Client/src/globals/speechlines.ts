export const SpeechLines = {
  approachingEn: (stopName: string) => `Now approaching ${stopName}.`,
  approachingEs: (stopName: string) => `Próxima parada: ${stopName}.`,
  doorsOpenEn: 'Doors open. Watch your step.',
  doorsOpenEs: 'Puertas abiertas. Cuidado al bajar.',
  doorsClosingEn: 'Doors closing.',
  doorsClosingEs: 'Puertas cerrando.',
} as const
