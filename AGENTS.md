# NotificationSystem Agents

This repository is a new microservice responsible for application notifications.

## Goals
- Persist all in-application notifications.
- Send notifications via email, SMS, WhatsApp, and push.
- Process events from other services (e.g., Claims) via RabbitMQ + MassTransit.
- Emit any necessary acknowledgment or status events after processing.

## Architecture Notes
- Incoming domain events (ClaimStatusChanged, etc.) are consumed from RabbitMQ.
- MassTransit is the messaging library for consumers, retries, and outbox/inbox.
- Notification processing is asynchronous; persistence occurs before external sends.

## Conventions
- Events are immutable, versioned, and backwards compatible.
- Idempotency is required for all consumers.
- External providers must be abstracted behind interfaces.
- All notifications must be auditable (status, attempts, timestamps).

## Directory Expectations
- `src/` contains the service implementation.
- `lib/` contains shared or generated code.
- `tests/` contains unit/integration tests.

