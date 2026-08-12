import { SpeechLines } from '../globals/speechlines'

//tts reads "St" as "st" and mumbles the "&", so expand the usual street abbreviations before speaking
const AbbreviationWords: Record<string, string> = {
  st: 'Street',
  ave: 'Avenue',
  blvd: 'Boulevard',
  pkwy: 'Parkway',
  pl: 'Place',
  dr: 'Drive',
  rd: 'Road',
  ln: 'Lane',
  hwy: 'Highway',
  ct: 'Court',
  sq: 'Square',
  ter: 'Terrace',
}

export function expandStopName(name: string, conjunction = 'and'): string {
  return name
    .replace(/\s*[&/]\s*/g, ` ${conjunction} `)
    .split(/\s+/)
    .map((word) => {
      //digits stay out of the prefix so ordinals like 3rd or 12th never match the rd/th lookup
      const parts = word.match(/^([^A-Za-z0-9]*)([A-Za-z]+)\.?([^A-Za-z]*)$/)
      if (!parts) return word
      const expanded = AbbreviationWords[parts[2].toLowerCase()]
      return expanded ? `${parts[1]}${expanded}${parts[3]}` : word
    })
    .join(' ')
    .trim()
}

function pickFemaleVoice(langPrefix: string): SpeechSynthesisVoice | null {
  const voices = window.speechSynthesis.getVoices()
  const matches = voices.filter((v) => v.lang.toLowerCase().startsWith(langPrefix))
  const female = matches.find((v) =>
    /female|samantha|victoria|karen|moira|tessa|fiona|zira|susan|linda|paulina|monica|sabina|google.*español|google.*spanish/i.test(
      `${v.name} ${v.voiceURI}`,
    ),
  )
  return female ?? matches[0] ?? null
}

function speak(text: string, lang: string): Promise<void> {
  return new Promise((resolve) => {
    if (!('speechSynthesis' in window)) {
      resolve()
      return
    }

    const utterance = new SpeechSynthesisUtterance(text)
    utterance.lang = lang
    const voice = pickFemaleVoice(lang.slice(0, 2).toLowerCase())
    if (voice) utterance.voice = voice
    utterance.rate = 0.95
    utterance.onend = () => resolve()
    utterance.onerror = () => resolve()
    window.speechSynthesis.speak(utterance)
  })
}

let chain: Promise<void> = Promise.resolve()

async function ensureVoices(): Promise<void> {
  if (window.speechSynthesis.getVoices().length === 0) {
    await new Promise<void>((resolve) => {
      window.speechSynthesis.onvoiceschanged = () => resolve()
      setTimeout(resolve, 300)
    })
  }
}

export function announceBilingual(english: string, spanish: string): Promise<void> {
  chain = chain.then(async () => {
    await ensureVoices()
    await speak(english, 'en-US')
    await speak(spanish, 'es-US')
  })

  return chain
}

function announceBilingualWithSpecial(english: string, spanish: string, special: string, onSpecialStart?: () => void, onSpecialEnd?: () => void): Promise<void> {
  chain = chain.then(async () => {
    await ensureVoices()
    await speak(english, 'en-US')
    await speak(spanish, 'es-US')

    onSpecialStart?.()
    try {
      await speak(special, 'en-US')
      await speak(special, 'es-US')
    } finally {
      onSpecialEnd?.()
    }
  })

  return chain
}

export function announcePhase(phase: string, stopName: string | null | undefined, specialAlert?: string | null, onSpecialStart?: () => void, onSpecialEnd?: () => void): Promise<void> {
  const raw = stopName?.trim() || 'the next stop'
  const nameEn = expandStopName(raw, 'and')
  const nameEs = expandStopName(raw, 'y')

  if (phase === 'approaching') {
    const special = specialAlert?.trim()
    if (special) {
      return announceBilingualWithSpecial(
        SpeechLines.approachingEn(nameEn),
        SpeechLines.approachingEs(nameEs),
        special,
        onSpecialStart,
        onSpecialEnd,
      )
    }
    return announceBilingual(SpeechLines.approachingEn(nameEn), SpeechLines.approachingEs(nameEs))
  }

  if (phase === 'doorsOpen') {
    return announceBilingual(SpeechLines.doorsOpenEn, SpeechLines.doorsOpenEs)
  }

  if (phase === 'doorsClosing') {
    return announceBilingual(SpeechLines.doorsClosingEn, SpeechLines.doorsClosingEs)
  }

  return Promise.resolve()
}
