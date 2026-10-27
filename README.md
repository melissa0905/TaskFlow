# TaskFlow — AI Destekli Proje ve Görev Yönetimi

TaskFlow; React, ASP.NET Core ve Python ile geliştirilen, **Google Gemini destekli görev planlama** özelliğine sahip bir proje yönetimi uygulamasıdır. Projeleri ve görevleri yönetin; bir hedef belirleyerek projenizin bilgilerine dayalı, uygulanabilir görev önerileri üretin.

## AI ile Hedeften Görev Planına

Python/FastAPI servisi, Gemini'nin function calling özelliğini kullanarak seçilen projenin bilgilerini .NET API'den alır ve kullanıcının hedefine uygun bir plan oluşturur.

1. Kullanıcı proje kimliğini, hedefini ve istediği en fazla görev sayısını gönderir.
2. Gemini `get_project` aracını çağırır; Python, istenen kimliğin seçilen projeyle eşleştiğini doğrular ve proje adını/açıklamasını .NET API'den getirir.
3. Araç sonucu Gemini'ye iletilir; model Türkçe bir özet ve görev önerileri üretir.
4. Yanıt Pydantic ile doğrulanır ve JSON olarak döndürülür.

Her öneri bir başlık, açıklama ve `backend`, `frontend`, `testing` veya `devops` kategorisi içerir. İstek başına 1–10 görev önerilebilir. Servis proje adı ve açıklamasını kullanır; kaynak kodu veya mevcut görevleri okumaz. Üretilen plan öneri olarak döner, görevleri veritabanına otomatik kaydetmez.

AI özelliği şu anda ayrı bir HTTP API üzerinden kullanılabilir. Frontend'in mevcut `/api` proxy'si .NET API'ye yönelir; AI servisini aşağıdaki örnekte olduğu gibi doğrudan çağırabilirsiniz.

## Teknolojiler ve Mimari

| Katman | Teknolojiler | Sorumluluk |
| --- | --- | --- |
| Frontend | React 19, TypeScript, Vite | Kullanıcı arayüzü |
| Backend | ASP.NET Core, .NET 10, EF Core, PostgreSQL | Proje ve görev verileri |
| AI servisi | Python, FastAPI, Google Gen AI SDK, Pydantic | Gemini araç çağrıları ve doğrulanmış görev planları |
| Mesajlaşma | RabbitMQ, MassTransit, SignalR | Olay işleme ve canlı bildirimler |

```text
TaskFlow/
  ai-service/  Python + FastAPI + Gemini görev planlama servisi
  backend/     ASP.NET Core API, application, domain, infrastructure ve worker
  frontend/    React + TypeScript + Vite istemcisi
```

## Yerel Kurulum

Gereksinimler: .NET 10 SDK, Docker, Node.js/npm, Python 3.11 veya üzeri ve Gemini API anahtarı. Komutları depo kökünden başlayarak çalıştırın; backend, AI servisi ve frontend için ayrı terminaller kullanın.

### 1. Backend ve Altyapı

PostgreSQL ve RabbitMQ servislerini başlatın:

```powershell
cd backend
docker compose up -d
```

API'yi başlatmadan önce `ConnectionStrings:TaskFlowDb` bağlantı dizesini user secrets veya yerel yapılandırmada tanımlayın. Veritabanı şemasının EF Core migration'larıyla oluşturulduğundan emin olun.

```powershell
dotnet run --project src/TaskFlow.Api
```

Geliştirme ortamındaki HTTP adresi: `http://localhost:5042`.

### 2. Gemini AI Servisi

Windows / PowerShell:

```powershell
cd ai-service
python -m venv .venv
.\.venv\Scripts\python.exe -m pip install -r requirements.txt
Copy-Item app/.env.example .env
```

Mevcut bir `.env` dosyanız varsa kopyalama adımını atlayın. Servis, **`ai-service/.env`** dosyasını okur. Aşağıdaki değerleri bu dosyada ayarlayın:

```dotenv
GEMINI_API_KEY=your_gemini_api_key
GEMINI_MODEL=gemini-3.8-flash
DOTNET_API_BASE_URL=http://127.0.0.1:5042
```

`GEMINI_MODEL` hesabınızın erişebildiği model kimliği olmalıdır. Ortam değişkeni verilmezse kodun varsayılan modeli `gemini-2.5-flash` olur; örnek ortam dosyası `gemini-3.8-flash` kullanır. API anahtarınızı depoya eklemeyin. Ayarları değiştirdikten sonra AI servisini yeniden başlatın.

```powershell
.\.venv\Scripts\python.exe -m uvicorn app.main:app --reload --port 8000
```

- Swagger UI: `http://127.0.0.1:8000/docs`
- Sağlık kontrolü: `GET http://127.0.0.1:8000/health`
- Görev planı: `POST http://127.0.0.1:8000/api/task-plans`

Sağlık kontrolü servisin ayakta olduğunu gösterir; Gemini veya .NET API bağlantısını test etmez.

### 3. Frontend

```powershell
cd frontend
npm install
npm run dev
```

Tarayıcıda Vite'ın terminalde gösterdiği adresi açın. Frontend'in `/api` ve `/hubs` istekleri `http://localhost:5042` adresindeki .NET API'ye yönlendirilir.

## AI Görev Planı Örneği

Önce .NET API'deki `GET /api/projects` yanıtından mevcut bir proje kimliği alın. AI servisinin Swagger UI'ında `POST /api/task-plans` için aşağıdaki gövdeyi kullanın; `project_id` değerini gerçek proje kimliğiyle değiştirin:

```json
{
  "project_id": "00000000-0000-0000-0000-000000000001",
  "goal": "Proje için görev oluşturma ve listeleme akışını geliştir.",
  "max_tasks": 3
}
```

Yanıt biçimi aşağıdaki gibidir. İçerik ve görev sayısı modele ve isteğe göre değişir:

```json
{
  "project_id": "00000000-0000-0000-0000-000000000001",
  "summary": "Görev oluşturma ve listeleme için geliştirme planı.",
  "tasks": [
    {
      "title": "Görev oluşturma doğrulamalarını uygula",
      "description": "Zorunlu alanları doğrula. Geçersiz istekler açıklayıcı hata döndürmeli, geçerli istekler kaydedilmelidir.",
      "category": "backend"
    }
  ]
}
```

## AI Hatalarını Anlama

| HTTP durumu | Olası neden |
| --- | --- |
| `404` | Seçilen proje bulunamadı. |
| `422` | İstek doğrulanamadı; UUID, hedef uzunluğu ve `max_tasks` değerini kontrol edin. |
| `502` | .NET/Gemini bağlantısı başarısız, Gemini isteği reddedilmiş veya üretilen plan doğrulanamamış olabilir. Yanıtın `detail` alanını ve servis loglarını inceleyin. |
| `503` | API anahtarı eksik, model erişimi kullanılamıyor, kota/istek sınırı aşılmış veya Gemini yoğun olabilir. `detail` alanı nedeni açıklar. |
| `504` | Plan üretimi veya Gemini isteği zaman aşımına uğradı. |

Gemini'nin yoğunluk yanıtı, tek başına API anahtarınızın yanlış olduğu anlamına gelmez. Proje bilgisinin alınabilmesi için .NET API'nin de çalışıyor olması gerekir.

## Geliştirme Komutları

Frontend için `frontend` dizininde:

```powershell
npm run build
npm run lint
```

`bin`, `obj`, `dist` ve `node_modules` gibi derleme çıktıları depoda tutulmaz.
