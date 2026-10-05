import { useCallback, useEffect, useState } from "react";
import { getWorkTasks } from "../api/workTasks";
import type { WorkTask } from '../types/workTask'


type UseTasksParams = {
    search: string;
    status: number | undefined;
    page: number;
    pageSize: number;
    enabled?: boolean;
};

export function useTasks({
    search,
    status,
    page,
    pageSize, enabled = true
}: UseTasksParams) {
    const [tasks, setTasks] = useState<WorkTask[]>([])
    const [totalCount, setTotalCount] = useState(0);
    const [totalPages, setTotalPages] = useState(0);
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('');
    ""
    const reload = useCallback(async (signal?: AbortSignal): Promise<void> => {
        setLoading(true)
        setError('')
        try {
            const result = await getWorkTasks(
                { search, status, page, pageSize },
                signal,
            );

            if (!signal?.aborted) {
                setTasks(result.items);
                setTotalCount(result.totalCount);
                setTotalPages(result.totalPages);
            }
        } catch (error: unknown) {
            if (!signal?.aborted) {
                setError(
                    error instanceof Error
                        ? error.message
                        : 'Beklenmeyen bir hata oluştu.',
                );
            }
        } finally {
            if (!signal?.aborted) {
                setLoading(false);
            }
        }
    }, [search, status, page, pageSize],)

    useEffect(() => {
        if (!enabled) {
            return;
        }
        const controller = new AbortController();
        void reload(controller.signal);
        return () => controller.abort();
    }, [reload])
    return {
        tasks,
        totalCount,
        totalPages,
        loading,
        error,
        reload,
    };
}