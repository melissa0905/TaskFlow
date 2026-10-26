import { useEffect, useState } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';

export function useNotificationConnection(
  recipientId: string,
  onChanged: () => void,
) {
  const [connectionError, setConnectionError] = useState('');

  useEffect(() => {
    setConnectionError('');

    if (!recipientId) {
      return;
    }

    let disposed = false;

    const connection = new HubConnectionBuilder()
      .withUrl('/hubs/notifications')
      .withAutomaticReconnect()
      .build();

    connection.on('NotificationsChanged', () => {
      if (!disposed) {
        onChanged();
      }
    });

    async function subscribe() {
      await connection.invoke('Subscribe', recipientId);

      if (!disposed) {
        setConnectionError('');

        // Bağlantı kopukken kaçan değişiklikleri de al.
        onChanged();
      }
    }

    connection.onreconnecting(() => {
      if (!disposed) {
        setConnectionError(
          'Canlı bağlantı kesildi, yeniden bağlanılıyor...',
        );
      }
    });

    connection.onreconnected(async () => {
      if (disposed) {
        return;
      }

      try {
        await subscribe();
      } catch {
        if (!disposed) {
          setConnectionError(
            'Canlı bildirim aboneliği kurulamadı. Sayfayı yenile.',
          );
        }
      }
    });

    connection.onclose(() => {
      if (!disposed) {
        setConnectionError(
          'Canlı bağlantı kapandı. Sayfayı yenileyebilirsin.',
        );
      }
    });

    async function connect() {
      try {
        await connection.start();

        if (disposed) {
          return;
        }

        await subscribe();
      } catch {
        if (!disposed) {
          setConnectionError(
            'Canlı bağlantı kurulamadı. Sayfayı yenileyebilirsin.',
          );
        }
      }
    }

    void connect();

    return () => {
      disposed = true;

      void connection.stop().catch(() => {
        // Sayfadan ayrılırken bağlantı zaten kapanmış olabilir.
      });
    };
  }, [recipientId, onChanged]);

  return { connectionError };
}