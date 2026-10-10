import type { PagedResult } from "../types/pagedResult";
import type { CreateWorkTaskInput, GetWorkTasksParams, WorkTask, WorkTaskDetail } from "../types/workTask";
import { readApiError } from "./ApiError";

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

export async function updateWorkTask(id: string, input: Partial<CreateWorkTaskInput>): Promise<void> {
    const response = await fetch(`/api/work-tasks/${id}`, {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(input),
    })
    if (!response.ok) {
        throw await readApiError(
            response,
            'Görev oluşturulamadı.',
        );
    }
}

export async function deleteWorkTask(id: string): Promise<void> {
    const response = await fetch(`/api/work-tasks/${id}`, {
        method: 'DELETE',
    })

    if (!response.ok) {
        throw new Error('Görev silinemedi.')
    }
}
export async function getWorkTasks(
    {
        search,
        status,
        page = 1,
        pageSize = 10,
    }: GetWorkTasksParams = {},
    signal?: AbortSignal,
): Promise<PagedResult<WorkTask>> {
    const params = new URLSearchParams({
        page: String(page),
        pageSize: String(pageSize),
    });

    if (search?.trim()) {
        params.set('search', search.trim());
    }

    if (status !== undefined) {
        params.set('status', String(status));
    }

    const response = await fetch(
        `/api/work-tasks?${params.toString()}`,
        { signal },
    );

    if (!response.ok) {
        throw new Error('Görevler yüklenemedi.');
    }

    return response.json();
}