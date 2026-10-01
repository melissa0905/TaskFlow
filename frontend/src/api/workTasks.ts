import type { CreateWorkTaskInput, WorkTask, WorkTaskDetail } from "../types/workTask";

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

export async function getWorkTaskById(id: string, signal?: AbortSignal): Promise<WorkTaskDetail> {
    const response = await fetch(`/api/work-tasks/${id}`, { signal })

    if (response.status === 404) {
        throw new Error('Görev bulunamadı.')
    }

    if (!response.ok) {
        throw new Error('Görev bilgileri yüklenemedi.')
    }

    return response.json() as Promise<WorkTaskDetail>
}