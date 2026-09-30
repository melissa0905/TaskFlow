import type { CreateWorkTaskInput, WorkTask } from "../types/workTask";

export async function createWorkTask(input: CreateWorkTaskInput): Promise<void> {
    const response = await fetch('/api/work-tasks', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(input),
    });
    if (!response.ok) {
        throw new Error('İş görevi oluşturulamadı.')
    }   
}

export async function getWorkTasks(signal?: AbortSignal): Promise<WorkTask[]> {
  const response = await fetch('/api/work-tasks', { signal })

  if (!response.ok) {
    throw new Error('Görevler yüklenemedi.')
  }

  return response.json() as Promise<WorkTask[]>
}
export async function changeWorkTaskStatus(
  id: string,
  status: number,
): Promise<void> {
  const response = await fetch(`/api/work-tasks/${id}/status`, {
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ status }),
  })

  if (!response.ok) {
    throw new Error('Görev durumu güncellenemedi.')
  }
}