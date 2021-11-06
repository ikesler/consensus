## What is this?

A tool for gathering data from external sources (e.g., social networks) and saving it to Elasticsearch document DB.

## Why is it called "Consensus"?

"ConSensus" was the name of onboard interactive knowledge base in Blindsight science fiction novell by Peter Watts.

## What data sources are supported?

Only vk.com for now.

## How to run it locally?
- Back end: go to `back` directory, open Visual Studio solution and run it as usual. In order to populate data source secrets and other configs: rename `dev_secrets.example.json` to `dev_secrets.json` and populate all the values. Then run `dotnet user-secrets clear` and `type .\dev_secrets.json | dotnet user-secrets set`.
- Front end: go to `front` directory, then `npm i`, then `npm run serve`. Depending on the port allocated for the back end, you might need to adjust `.env.development` accordingly.

## How to deploy?

Rename `.env.example` to `.env` and populate all the variable. Then run `deploy.sh`

