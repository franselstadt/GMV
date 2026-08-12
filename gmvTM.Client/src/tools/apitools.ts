async function parseError(response: Response): Promise<string> {
  try {
    const body = await response.json()
    if (Array.isArray(body?.errors) && body.errors.length)
      return body.errors.join('; ')
    if (body?.title) return body.title
    if (body?.message) return body.message
  } catch {
  }
  return response.statusText || `Request failed (${response.status})`
}

const BasicAuthHeader = `Basic ${btoa('ladot:dieengele')}`

export async function api<T>(path: string, init?: RequestInit): Promise<T> {
  const headers: HeadersInit = {
    Accept: 'application/json',
    Authorization: BasicAuthHeader,
    ...(init?.headers ?? {}),
  }
  if (init?.body && !(init.body instanceof FormData)) {
    ;(headers as Record<string, string>)['Content-Type'] = 'application/json'
  }

  const response = await fetch(`/api/v1${path}`, { ...init, headers })
  if (!response.ok) throw new Error(await parseError(response))
  if (response.status === 204) return undefined as T
  const text = await response.text()
  return text ? (JSON.parse(text) as T) : (undefined as T)
}

export function routePath(routeCode: string, suffix = ''): string {
  const code = encodeURIComponent(routeCode.trim())
  return `/routes/${code}${suffix}`
}
