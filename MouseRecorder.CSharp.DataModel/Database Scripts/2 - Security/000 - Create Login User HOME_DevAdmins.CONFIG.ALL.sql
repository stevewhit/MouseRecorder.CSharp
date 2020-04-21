IF NOT EXISTS (SELECT * FROM master.dbo.syslogins WHERE [name] = 'HOME\DevAdmins')
BEGIN
	PRINT 'Creating "HOME\DevAdmins" login..'

	CREATE LOGIN [HOME\DevAdmins] FROM WINDOWS WITH DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english]
	ALTER SERVER ROLE [sysadmin] ADD MEMBER [HOME\DevAdmins]
END
ELSE
	PRINT 'The user "HOME\DevAdmins" login already exists..'
