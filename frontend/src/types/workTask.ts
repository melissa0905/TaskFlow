export type CreateWorkTaskInput = {
  projectId: string
  title: string
  description: string
  dueDate: string
  assignedToId: string | null
}

export type WorkTask = {
  id: string
  title: string
  description: string
  status: number
  dueDate: string
  projectId: string
  projectName: string
  assignedToId: string | null
  assigneeName: string | null
}

export type WorkTaskDetail = {
  id: string
  title: string
  description: string
  status: number
  dueDate: string
  projectId: string
  assignedToId: string | null
}

export type UpdateWorkTaskInput = {
  title?: string
  description?: string
  status?: number
  dueDate?: string
  assignedToId?: string | null
}