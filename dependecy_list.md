## Dependencies for MongoDB Driver

### Blockcore.Indexer.core:
- **Client**
  - **Types**
    - `peerInfo.cs`: `MongoDB.Bson.Serialization.Attributes`
- **Sync**
  - **SyncTasks**
    - `BlockIndexer.cs`: `MongoDB.Bson`, `MongoDB.Driver`
    - `RichListSync.cs`: `MongoDB.Bson`, `MongoDB.Driver`
  - `SyncOperations.cs`: `MongoDB.Driver`
- `Startup.cs`: `MongoDB.Driver`

## Dependencies on `Storage.Mongo` Namespace

### Blockcore.Indexer.core:
- **Controllers**
  - `insightController`
  - `QueryController`
  - `StatsController`
- **Models**
  - `QueryOrphanBlocks`
- **Operations**
  - **Types**
    - `InsertStats.cs`
    - `StorageBatch.cs`
    - `UtxoCache.cs`
  - `IUtxoCache.cs`
- **Settings**
  - `IndexerSettings.cs`
- **Storage**
  - `IMapMongoBlockToStorage`
  - `IStorage.cs`
  - `MapMongoBlockToStorage`
- **Sync**
  - **SyncTasks**
    - `BlockIndexer.cs`
    - `BlockPuller.cs`
    - `BlockStartup.cs`
    - `BlockStore.cs`
    - `RichListSync.cs`
    - `StatsSync.cs`
  - `SyncOperations.cs`
- `Startup.cs`
