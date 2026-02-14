# NotificationSystem TODO

## Goal: generic notification microservice
- [ ] Define inbound contract `NotificationRequestedV1` (shared package or `lib/`).
- [ ] Document required fields for idempotency and audit (`SourceService`, `SourceEventId`, `CorrelationId`).

## Inbound processing (consume + persist)
- [ ] Implement consumer for `NotificationRequestedV1` (MassTransit).
- [ ] Idempotency check on `Notification.SourceService + SourceEventId`.
- [ ] Persist `Notification`, `NotificationRecipient`, `NotificationDelivery` before any sends.
- [ ] Publish `NotificationDeliveryEnqueuedV1` per recipient/channel.

## Delivery pipeline
- [ ] Keep `NotificationDeliveryEnqueuedConsumer` to mark delivery as sending.
- [ ] Add provider interfaces (Email/SMS/WhatsApp/Push).
- [ ] Add stub adapters and update delivery status (Sent/Failed/Retrying).

## Client access
- [ ] Add query API to list notifications per recipient.
- [ ] Add API to mark notifications as read.
- [ ] (Optional) Add SignalR/WebSocket for real-time in-app updates.

## Ops and tests
- [ ] Add health checks for DB and RabbitMQ.
- [ ] Add unit tests for consumer + idempotency.
- [ ] Add integration test covering end-to-end persistence.
