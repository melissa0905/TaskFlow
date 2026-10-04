import type { DashboardSummary } from "../types/dashboard";


export async function getDashboardSummary(signal?: AbortSignal): Promise<DashboardSummary> {
    const response = await fetch('/api/dashboard/summary', { signal })  
    if (!response.ok) {
        throw new Error('Failed to fetch dashboard summary');
    }
    return response.json();
}       