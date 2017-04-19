_Tweak hostnames & ports depending on your actual environment setup_

```
# To access local development API
ECHECK_API_ENDPOINT=http://localhost:49390/api
# To access integration API
# ECHECK_API_ENDPOINT=https://echeck-int.crossroads.net/proxy/SignInCheckIn/api

# To access local development CMS
CRDS_CMS_CLIENT_ENDPOINT=http://localhost:81/
# To access integration CMS
# CRDS_CMS_CLIENT_ENDPOINT=https://contentint.crossroads.net/

# Domain-locked API key - this will vary based on what ECHECK_API_ENDPOINT (local or int) is being used
ECHECK_API_TOKEN=[get appropriate value from MinistryPlatform "Client API Keys" table]
```
