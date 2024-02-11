CREATE TABLE DamageLog (
    uniqueId        BIGINT NOT NULL,
    actorSide       INT NOT NULL, 
    actorId         INT NOT NULL,
    targetSide      INT NOT NULL, 
    targetId        INT NOT NULL,
    attackId        INT NOT NULL,
    hp              FLOAT NOT NULL,
    lp              FLOAT NOT NULL,
    damageHp        FLOAT NOT NULL,
    damageLp        FLOAT NOT NULL,
    gameTime        FLOAT NOT NULL
);