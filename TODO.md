# NotificationSystem TODO

## Core design
- [ ] Define message contracts for inbound events (e.g., ClaimStatusChanged).
- [ ] Define notification payload model (recipient, channels, template, metadata).
- [ ] Define persistence schema for in-app notifications and delivery audit trail.
- [ ] Define idempotency strategy (message id + recipient + channel).

## Messaging (RabbitMQ + MassTransit)
- [ ] Configure RabbitMQ connection settings and exchanges.
- [ ] Add MassTransit configuration (consumers, retries, outbox/inbox).
- [ ] Implement consumers for incoming domain events.
- [ ] Add dead-letter handling and poison message strategy.

## Notification pipeline
- [ ] Implement notification router (choose channels per event/recipient).
- [ ] Implement persistence of in-app notifications before sending.
- [ ] Implement delivery state machine (Queued, Sent, Failed, Retrying).
- [ ] Add provider abstractions (Email, SMS, WhatsApp, Push).
- [ ] Implement initial provider adapters (stubs or real integrations).

## Events back out
- [ ] Emit NotificationProcessed / NotificationFailed events when done.
- [ ] Add correlation IDs for traceability across services.

## Templates and localization
- [ ] Define template storage and rendering mechanism.
- [ ] Add localization support if required (culture, timezone).

## Security and compliance
- [ ] Store secrets in environment/secret manager.
- [ ] PII handling policy for notification data.
- [ ] Audit logging for all sends and failures.

## Observability and ops
- [ ] Add structured logging and metrics.
- [ ] Add health checks (RabbitMQ, database).
- [ ] Add dashboards and alerting plan.

## Testing
- [ ] Unit tests for consumers and routing rules.
- [ ] Integration tests with RabbitMQ and database.
- [ ] Contract tests for event schemas.

