-- Extension to auto generate uuids
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE reminders (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id NUMERIC(20, 0) NOT NULL,
    channel_id NUMERIC(20, 0) NOT NULL,
    due_date TIMESTAMP NOT NULL,
    message TEXT NOT NULL
);
