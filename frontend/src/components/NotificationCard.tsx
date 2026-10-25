import { Link } from 'react-router';
import type { NotificationListItem } from '../types/notification';

type NotificationCardProps = {
    notification: NotificationListItem;
    disabled: boolean;
    updating: boolean;
    onMarkAsRead: (id: string) => Promise<void>;
};
export default function NotificationCard({
    notification,
    disabled,
    updating,
    onMarkAsRead,
}: NotificationCardProps) {
    return (
        <li
            className={[
                'notification-card',
                notification.isRead
                    ? 'notification-card--read'
                    : 'notification-card--unread',
            ].join(' ')}
        >
            <div className="notification-card__heading">
                <span className="notification-badge">
                    {notification.isRead ? 'Okundu' : 'Yeni'}
                </span>

                <time dateTime={notification.createdAtUtc}>
                    {new Date(notification.createdAtUtc).toLocaleString('tr-TR')}
                </time>
            </div>

            <p className="notification-card__message">
                {notification.message}
            </p>

            <div className="notification-card__actions">
                <Link to={`/tasks/${notification.taskId}`}>
                    Göreve git →
                </Link>

                {!notification.isRead && (
                    <button
                        type="button"
                        disabled={disabled}
                        onClick={() => {
                            void onMarkAsRead(notification.id);
                        }}
                    >
                        {updating ? 'İşleniyor...' : 'Okundu işaretle'}
                    </button>
                )}
            </div>
        </li>

    );
}