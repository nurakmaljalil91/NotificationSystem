# NotificationSystem TODO

## Goal: generic notification microservice
- [x] Define inbound contract `NotificationRequestedV1` (shared package or `lib/`).
- [x] Document required fields for idempotency and audit (`SourceService`, `SourceEventId`, `CorrelationId`).

## Inbound processing (consume + persist)
- [x] Implement consumer for `NotificationRequestedV1` (MassTransit).
- [x] Idempotency check on `Notification.SourceService + SourceEventId`.
- [x] Persist `Notification`, `NotificationRecipient`, `NotificationDelivery` before any sends.
- [x] Publish `NotificationDeliveryEnqueuedV1` per recipient/channel.

## Delivery pipeline
- [x] Keep `NotificationDeliveryEnqueuedConsumer` to mark delivery as sending.
- [x] Add provider interfaces (Email/SMS/WhatsApp/Push).
- [x] Add stub adapters and update the delivery status (Sent/Failed/Retrying).

## Client access
- [x] Add query API to list notifications per recipient.
- [x] Add API to mark notifications as read.
- [x] (Optional) Add SignalR/WebSocket for real-time in-app updates.

## Ops and tests
- [x] Add health checks for DB and RabbitMQ.
- [x] Add unit tests for consumer + idempotency.
- [x] Add integration test covering end-to-end persistence.
