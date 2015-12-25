# Azure Queue example application

TBD

## Implementation

### User Secrets and appsettings.json used in configuration:

The application uses configuration to access Azure information in the following order:
- `appsettings.json`
- user secrets 
- environment variables

The configuration file or user secrets section-based keys:

```
user-secret list
info: Azure:Storage:AccountName = ...
info: Azure:Storage:AccountKey = ....
info: Azure:Storage:QueueName = queue-name
```

## Author
@peterblazejewicz