powershell -ExecutionPolicy RemoteSigned -File createInsert.ps1 -tableName PlatoonLog -prefix platoon_log
powershell -ExecutionPolicy RemoteSigned -File createInsert.ps1 -tableName DamageLog -prefix damage_log
