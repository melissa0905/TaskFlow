import { useEffect, useState } from "react";
import type { DashboardSummary } from "../types/dashboard";
import { getDashboardSummary } from "../api/getDashboardSummary";
import { PageHeader } from "../components/PageHeader";
import { Link } from "react-router";
import StatCard from "../components/StatCard";

export default function DashboardPage() {

    const [summary, setSummary] = useState<DashboardSummary | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const controller = new AbortController();

        async function loadDashboard() {
            try {
                const data = await getDashboardSummary(controller.signal);

                if (!controller.signal.aborted) {
                    setSummary(data);
                }


            } catch (error) {
                if (!controller.signal.aborted) {
                    setError(
                        error instanceof Error
                            ? error.message
                            : 'Beklenmeyen bir hata oluştu.',
                    );
                }
            } finally {
                if (!controller.signal.aborted) {
                    setLoading(false);
                }
            }
        }

        void loadDashboard();

        return () => {
            controller.abort();
        };
    }, []);
    7
    return (
        <main>
            <PageHeader
                title="Genel bakış"
                description="Görevlerinin durumunu tek ekrandan takip et."
            />

            {loading && <p role="status">Dashboard yükleniyor...</p>}

            {error && (
                <p className="dashboard-error" role="alert">
                    {error}
                </p>
            )}

            {!loading && !error && summary && (
                <>
                    <section
                        className="dashboard-grid"
                        aria-label="Görev istatistikleri"
                    >
                        <StatCard
                            title="Toplam görev"
                            value={summary.totalTasks}
                            description="Sistemde kayıtlı bütün görevler"
                            tone="primary"
                        />

                        <StatCard
                            title="Devam eden"
                            value={summary.inProgressTasks}
                            description="Üzerinde çalışılan görevler"
                            tone="warning"
                        />

                        <StatCard
                            title="Tamamlanan"
                            value={summary.completedTasks}
                            description="Bitirilen görevler"
                            tone="success"
                        />

                        <StatCard
                            title="Geciken"
                            value={summary.overdueTasks}
                            description="Son tarihi geçmiş, tamamlanmamış görevler"
                            tone="danger"
                        />
                    </section>

                    <section className="dashboard-shortcut">
                        <div>
                            <h2>Görevlerine devam et</h2>
                            <p>Görev oluştur, durumları güncelle ve detayları incele.</p>
                        </div>

                        <Link className="dashboard-link" to="/tasks">
                            Görevleri görüntüle →
                        </Link>
                    </section>
                </>
            )}
        </main>
    );

}