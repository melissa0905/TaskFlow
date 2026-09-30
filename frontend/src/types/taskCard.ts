import type { WorkTask } from "./workTask"

export type TaskCardProps = {
  task: WorkTask
  selected: boolean
  updating: boolean
  onSelect: (id: string) => void
  onStatusChange: (id: string, status: number) => Promise<void>
}