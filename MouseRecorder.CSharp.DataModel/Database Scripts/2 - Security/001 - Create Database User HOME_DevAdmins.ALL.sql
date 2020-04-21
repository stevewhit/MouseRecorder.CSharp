IF NOT EXISTS (SELECT * FROM sys.sysusers WHERE [name] = 'HOME\DevAdmins')
BEGIN
	PRINT 'Creating "HOME\DevAdmins" user"..'

	CREATE USER [HOME\DevAdmins] FOR LOGIN [HOME\DevAdmins]
	GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE TO [HOME\DevAdmins]
END
ELSE
	PRINT 'The user "HOME\DevAdmins" already exists..'
