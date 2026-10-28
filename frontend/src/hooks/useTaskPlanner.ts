import { useCallback, useEffect, useRef, useState } from 'react';

import { createTaskPlan } from '../api/taskPlans';

import type {
    CreateTaskPlanRequest,
    TaskPlanResponse,
} from '../types/taskPlan';

//Burada yeni kullandığımız useRef, devam eden isteğin AbortController nesnesini saklıyor.
//  Ref değişince render oluşmaz;
// ekranda gösterilecek loading ve error için ise state kullanıyoruz.

export function useTaskPlanner() {
    const [plan, setPlan] = useState<TaskPlanResponse | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const controllerRef = useRef<AbortController | null>(null);

    useEffect(() => {
        return () => {
            const controller = controllerRef.current;
            controllerRef.current = null;
            controller?.abort();
        };
    }, []);
    const generate = useCallback(
        async (request: CreateTaskPlanRequest): Promise<void> => {
            if (controllerRef.current) {
                return;
            }
            const controller = new AbortController();
            controllerRef.current = controller;

            setLoading(true);
            setError('');
            setPlan(null);

            let timedOut = false;

            const timeoutId = window.setTimeout(() => {
                timedOut = true;
                controller.abort();
            }, 130_000)

            try {
                const result = await createTaskPlan(request, controller.signal);
                if (controllerRef.current == controller) {
                    setPlan(result);
                }
            } catch (error: unknown) {
                if (controllerRef.current != controller) {
                    return;
                }
                if (controller.signal.aborted) {
                    setError(timedOut ? 'Yanıt bekleme süresi doldu.Daha sonra tekrar deneyebilirsin' : 'Plan oluşturma isteği iptal edildi.')
                }
                else {
                    setError(error instanceof Error ? error.message : 'Beklenmeyen bir hata oluştu.');
                }

            } finally {
                window.clearTimeout(timeoutId);

                if (controllerRef.current === controller) {
                    controllerRef.current = null;
                    setLoading(false);
                }
            }

        },
        [],
    );

    const cancel = useCallback(() => {
        controllerRef.current?.abort();
    }, []);
    const reset = useCallback(() => {
        setPlan(null);
        setError('');
    }, []);
    return { plan, loading, error, generate, cancel, reset, };
}