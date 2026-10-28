import { readApiError } from './ApiError';

import type {
    CreateTaskPlanRequest,
    TaskPlanResponse,
} from '../types/taskPlan';

export async function createTaskPlan(request: CreateTaskPlanRequest, signal: AbortSignal,): Promise<TaskPlanResponse> {
    const response = await fetch('/ai/api/task-plans', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
        signal,
    });

    if (!response.ok) {
        throw await readApiError(response,'Görev planı oluşturulmadı.');
    }
    return response.json();
}