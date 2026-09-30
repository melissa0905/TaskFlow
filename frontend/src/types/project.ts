export type Project = {
  id: string
  name: string
  description: string | null
  createdAtUtc: string
}

export type CreateProjectInput = {
  name: string
  description: string
}
