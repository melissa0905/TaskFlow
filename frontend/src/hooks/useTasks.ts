import { useCallback, useEffect, useState } from "react";
import { getWorkTasks } from "../api/workTasks";
import type { WorkTask } from '../types/workTask'

export function useTasks() {
    const [tasks, setTasks] = useState<WorkTask[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    const reload  = useCallback(async () => {
        setLoading(true)
        setError(null)
        try {
            setTasks(await getWorkTasks())
        } catch (err: unknown) {
            setError(err instanceof Error ? err.message : 'Görevler yüklenemedi.')
        } finally {
            setLoading(false)
        }
    }, [])
    useEffect(() => {
        void reload()
    }, [reload])
    return { tasks, loading, error, reload }
}