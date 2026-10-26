import { useCallback, useEffect, useState } from 'react';

import {
    getNotifications,
    markNotificationAsRead,
} from '../api/notifications';
import { getTeamMembers } from '../api/teamMembers';

import NotificationCard from '../components/NotificationCard';

import type { NotificationListItem } from '../types/notification';
import type { PagedResult } from '../types/pagedResult';
import type { TeamMember } from '../types/teamMember';
import { PageHeader } from '../components/PageHeader';
import { useNotificationConnection } from '../hooks/useNotificationConnection';

export default function NotificationsPage() {
    const [members, setMembers] = useState<TeamMember[]>([]);
    const [membersLoading, setMembersLoading] = useState(true);
    const [membersError, setMembersError] = useState('');

    const [recipientId, setRecipientId] = useState('');
    const [onlyUnread, setOnlyUnread] = useState(false);
    const [page, setPage] = useState(1);
    const [refreshKey, setRefreshKey] = useState(0);

    const [data, setData] =
        useState<PagedResult<NotificationListItem> | null>(null);

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [actionError, setActionError] = useState('');
    const [updatingId, setUpdatingId] = useState<string | null>(null);

    const handleNotificationsChanged = useCallback(() => {
        setRefreshKey((current) => current + 1);
    }, []);

    const { connectionError } = useNotificationConnection(
        recipientId,
        handleNotificationsChanged,
    );

    const pageSize = 5;

    useEffect(() => {
        let active = true;

        async function loadMembers() {
            try {
                const result = await getTeamMembers();

                if (active) {
                    setMembers(result);
                }
            } catch (error: unknown) {
                if (active) {
                    setMembersError(
                        error instanceof Error
                            ? error.message
                            : 'Ekip üyeleri yüklenemedi.',
                    );
                }
            } finally {
                if (active) {
                    setMembersLoading(false);
                }
            }
        }

        void loadMembers();

        return () => {
            active = false;
        };
    }, []);

    useEffect(() => {
        const controller = new AbortController();

        if (!recipientId) {
            setData(null);
            setLoading(false);
            setError('');
            return;
        }

        setLoading(true);
        setError('');

        async function loadNotifications() {
            try {
                const result = await getNotifications(
                    {
                        recipientId,
                        onlyUnread,
                        page,
                        pageSize,
                    },
                    controller.signal,
                );

                if (controller.signal.aborted) {
                    return;
                }

                const lastPage = Math.max(1, result.totalPages);

                if (page > lastPage) {
                    setPage(lastPage);
                    return;
                }

                setData(result);
            } catch (error: unknown) {
                if (!controller.signal.aborted) {
                    setError(
                        error instanceof Error
                            ? error.message
                            : 'Bildirimler yüklenemedi.',
                    );
                }
            } finally {
                if (!controller.signal.aborted) {
                    setLoading(false);
                }
            }
        }

        void loadNotifications();

        return () => {
            controller.abort();
        };
    }, [recipientId, onlyUnread, page, pageSize, refreshKey]);

    async function handleMarkAsRead(id: string): Promise<void> {
        if (!recipientId || updatingId !== null) {
            return;
        }

        setUpdatingId(id);
        setActionError('');

        try {
            await markNotificationAsRead(id, recipientId);

            setRefreshKey((current) => current + 1);
        } catch (error: unknown) {
            setActionError(
                error instanceof Error
                    ? error.message
                    : 'Bildirim güncellenemedi.',
            );
        } finally {
            setUpdatingId(null);
        }
    }

    const busy = loading || updatingId !== null;

    return (
        <main>
            <PageHeader
                title="Bildirimler"
                description="Görev atama bildirimlerini görüntüle ve takip et."
            />

            <section className="notification-toolbar">
                <label className="notification-recipient">
                    Ekip üyesi

                    <select
                        value={recipientId}
                        disabled={membersLoading || busy}
                        onChange={(event) => {
                            setRecipientId(event.target.value);
                            setPage(1);
                            setData(null);
                            setActionError('');
                        }}
                    >
                        <option value="">
                            {membersLoading
                                ? 'Ekip üyeleri yükleniyor...'
                                : 'Ekip üyesi seç'}
                        </option>

                        {members.map((member) => (
                            <option key={member.id} value={member.id}>
                                {member.fullName}
                            </option>
                        ))}
                    </select>
                </label>

                <label className="notification-unread-filter">
                    <input
                        type="checkbox"
                        checked={onlyUnread}
                        disabled={!recipientId || busy}
                        onChange={(event) => {
                            setOnlyUnread(event.target.checked);
                            setPage(1);
                            setData(null);
                            setActionError('');
                        }}
                    />

                    Yalnızca okunmamışlar
                </label>

                <button
                    type="button"
                    disabled={!recipientId || busy}
                    onClick={() => {
                        setActionError('');
                        setRefreshKey((current) => current + 1);
                    }}
                >
                    Yenile
                </button>
            </section>
            {connectionError && (
                <p role="status">{connectionError}</p>
            )}
            {membersError && (
                <p className="form-error" role="alert">
                    {membersError}
                </p>
            )}

            {error && (
                <p className="form-error" role="alert">
                    {error}
                </p>
            )}

            {actionError && (
                <p className="form-error" role="alert">
                    {actionError}
                </p>
            )}

            {!recipientId && !membersLoading && (
                <p>Bildirimlerini görmek için bir ekip üyesi seç.</p>
            )}

            {loading && (
                <p role="status">Bildirimler yükleniyor...</p>
            )}

            {recipientId && !loading && !error && data && (
                <>
                    <p className="task-result-count">
                        {data.totalCount} bildirim bulundu.
                    </p>

                    {data.items.length === 0 ? (
                        <div className="notification-empty">
                            {onlyUnread
                                ? 'Okunmamış bildirim bulunmuyor.'
                                : 'Henüz bildirim bulunmuyor.'}
                        </div>
                    ) : (
                        <ul className="notification-list">
                            {data.items.map((notification) => (
                                <NotificationCard
                                    key={notification.id}
                                    notification={notification}
                                    disabled={busy}
                                    updating={updatingId === notification.id}
                                    onMarkAsRead={handleMarkAsRead}
                                />
                            ))}
                        </ul>
                    )}

                    <div className="pagination">
                        <button
                            type="button"
                            disabled={busy || page <= 1}
                            onClick={() => {
                                setPage((current) => current - 1);
                            }}
                        >
                            ← Önceki
                        </button>

                        <span>
                            Sayfa {page} / {Math.max(1, data.totalPages)}
                        </span>

                        <button
                            type="button"
                            disabled={busy || page >= data.totalPages}
                            onClick={() => {
                                setPage((current) => current + 1);
                            }}
                        >
                            Sonraki →
                        </button>
                    </div>
                </>
            )}
        </main>
    );
}