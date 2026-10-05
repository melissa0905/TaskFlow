import { useCallback, useState } from 'react';

import { changeWorkTaskStatus } from '../api/workTasks';
import { useTasks } from '../hooks/useTasks';
import { PageHeader } from '../components/PageHeader';
import { CreateWorkTaskSection } from '../components/CreateWorkTaskSection';
import { TaskCard } from '../components/TaskCard';

export default function TasksPage() {
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [page, setPage] = useState(1);

  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [updatingId, setUpdatingId] = useState<string | null>(null);
  const [updateError, setUpdateError] = useState('');

  const pageSize = 5;

  const {
    tasks,
    totalCount,
    totalPages,
    loading,
    error,
    reload,
  } = useTasks({
    search,
    status:
      statusFilter === ''
        ? undefined
        : Number(statusFilter),
    page,
    pageSize,
  });

  const handleSelect = useCallback((id: string) => {
    setSelectedId((current) => (current === id ? null : id));
  }, []);

  const handleStatusChange = useCallback(
    async (id: string, status: number): Promise<void> => {
      setUpdatingId(id);
      setUpdateError('');

      try {
        await changeWorkTaskStatus(id, status);
        await reload();
      } catch (error: unknown) {
        setUpdateError(
          error instanceof Error
            ? error.message
            : 'Görev durumu güncellenemedi.',
        );
      } finally {
        setUpdatingId(null);
      }
    },
    [reload],
  );

  async function handleCreated(): Promise<void> {
    if (page !== 1) {
      setPage(1);
      return;
    }

    await reload();
  }

  const busy = loading || updatingId !== null;

  return (
    <main>
      <PageHeader
        title="Görevler"
        description="Görevlerini ara, filtrele ve durumlarını güncelle."
      />

      <CreateWorkTaskSection onCreated={handleCreated} />

      <section className="task-list-section">
        <div className="task-filters">
          <label>
            Görev ara
            <input
              type="search"
              placeholder="Görev başlığı..."
              value={search}
              disabled={updatingId !== null}
              onChange={(event) => {
                setSearch(event.target.value);
                setPage(1);
              }}
            />
          </label>

          <label>
            Durum
            <select
              value={statusFilter}
              disabled={updatingId !== null}
              onChange={(event) => {
                setStatusFilter(event.target.value);
                setPage(1);
              }}
            >
              <option value="">Tüm durumlar</option>
              <option value="1">Yapılacak</option>
              <option value="2">Devam ediyor</option>
              <option value="3">Tamamlandı</option>
            </select>
          </label>
        </div>

        {loading && <p role="status">Görevler yükleniyor...</p>}

        {error && <p role="alert">{error}</p>}

        {updateError && <p role="alert">{updateError}</p>}

        {!loading && !error && (
          <>
            <p className="task-result-count">
              Filtreye uygun toplam {totalCount} görev
            </p>

            {tasks.length === 0 ? (
              <p>Bu sayfada gösterilecek görev bulunamadı.</p>
            ) : (
              <ul className="task-list">
                {tasks.map((task) => (
                  <TaskCard
                    key={task.id}
                    task={task}
                    selected={selectedId === task.id}
                    updating={updatingId !== null}
                    onSelect={handleSelect}
                    onStatusChange={handleStatusChange}
                  />
                ))}
              </ul>
            )}
          </>
        )}

        <div className="pagination">
          <button
            type="button"
            disabled={busy || page <= 1}
            onClick={() => setPage((current) => current - 1)}
          >
            ← Önceki
          </button>

          <span>
            Sayfa {page} / {Math.max(1, totalPages)}
          </span>

          <button
            type="button"
            disabled={busy || page >= totalPages}
            onClick={() => setPage((current) => current + 1)}
          >
            Sonraki →
          </button>
        </div>
      </section>
    </main>
  );
}