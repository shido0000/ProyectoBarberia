// Migration: AddBarbershopModule
// Generated manually - Execute this SQL script to update the database

-- 1. Create BarbershopSubscriptionPlans table
CREATE TABLE IF NOT EXISTS BarbershopSubscriptionPlans (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT,
    MonthlyPrice REAL NOT NULL,
    MaxBarbers INTEGER,
    FeaturesJson TEXT,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 2. Create Barbershops table
CREATE TABLE IF NOT EXISTS Barbershops (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT,
    Address TEXT,
    Phone TEXT,
    LogoUrl TEXT,
    CoverImageUrl TEXT,
    BarbershopSubscriptionPlanId INTEGER NOT NULL,
    OwnerBarberId INTEGER NOT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (BarbershopSubscriptionPlanId) REFERENCES BarbershopSubscriptionPlans(Id) ON DELETE RESTRICT,
    FOREIGN KEY (OwnerBarberId) REFERENCES BarberProfiles(Id) ON DELETE RESTRICT
);

-- 3. Create BarbershopMembershipRequests table
CREATE TABLE IF NOT EXISTS BarbershopMembershipRequests (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    BarbershopId INTEGER NOT NULL,
    BarberId INTEGER NOT NULL,
    RequestDate TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Status TEXT NOT NULL DEFAULT 'Pending',
    ResponseDate TEXT,
    OwnerNotes TEXT,
    FOREIGN KEY (BarbershopId) REFERENCES Barbershops(Id) ON DELETE CASCADE,
    FOREIGN KEY (BarberId) REFERENCES BarberProfiles(Id) ON DELETE CASCADE
);

-- 4. Create Notifications table
CREATE TABLE IF NOT EXISTS Notifications (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    Message TEXT NOT NULL,
    Type TEXT NOT NULL DEFAULT 'Info',
    RelatedUrl TEXT,
    IsRead INTEGER NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

-- 5. Add CurrentBarbershopId column to BarberProfiles
ALTER TABLE BarberProfiles ADD COLUMN CurrentBarbershopId INTEGER;
CREATE INDEX IF NOT EXISTS IX_BarberProfiles_CurrentBarbershopId ON BarberProfiles(CurrentBarbershopId);

-- 6. Add BarbershopId column to Appointments
ALTER TABLE Appointments ADD COLUMN BarbershopId INTEGER;
CREATE INDEX IF NOT EXISTS IX_Appointments_BarbershopId ON Appointments(BarbershopId);

-- 7. Insert default barbershop subscription plans
INSERT INTO BarbershopSubscriptionPlans (Name, Description, MonthlyPrice, MaxBarbers, FeaturesJson, IsActive, CreatedAt)
VALUES 
    ('Inicial', 'Plan básico para barberías pequeñas', 20.00, 2, '{"features": ["Gestión de miembros", "Página pública", "Solicitudes de membresía"]}', 1, CURRENT_TIMESTAMP),
    ('Profesional', 'Plan para barberías en crecimiento', 50.00, 5, '{"features": ["Todo lo del plan Inicial", "Hasta 5 barberos", "Soporte prioritario"]}', 1, CURRENT_TIMESTAMP),
    ('Empresarial', 'Plan ilimitado para grandes barberías', 100.00, NULL, '{"features": ["Todo lo del plan Profesional", "Barberos ilimitados", "Soporte 24/7", "Características avanzadas"]}', 1, CURRENT_TIMESTAMP);

-- Note: After running this script, you may need to update existing data manually if needed.
-- The foreign key constraints will ensure data integrity.
