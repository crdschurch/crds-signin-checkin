_Tweak hostnames & ports depending on your actual environment setup_

```
# To access local development API
ECHECK_API_ENDPOINT=http://localhost:49390/api
# (or) To access integration API
# ECHECK_API_ENDPOINT=https://echeck-int.crossroads.net/proxy/SignInCheckIn/api

# To access integration CMS
# CRDS_CMS_CLIENT_ENDPOINT=https://contentint.crossroads.net/
# To access local development CMS
CRDS_CMS_CLIENT_ENDPOINT=http://localhost:81/

# Backend endpoint to connect to websockets
SIGNALR_ENDPOINT=http://checkin-gatewayint.crossroads.net/signalr
# (or) To access local backend
ECHECK_API_ENDPOINT=http://localhost:49390/signalr

# Domain-locked API key - this will vary based on what ECHECK_API_ENDPOINT (local or int) is being used
ECHECK_API_TOKEN=[get appropriate value from MinistryPlatform "Client API Keys" table]

# (Experimental) For use when you want to connect to int backend from local frontend. *This disables websockets* to avoid CORS connection issues
DISABLE_WEBSOCKETS=true
```
