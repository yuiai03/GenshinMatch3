# H??ng d?n PvP Mode v?i Photon

## T?ng quan

PvP Mode cho ph�p 2 ng??i ch?i ??u v?i nhau trong th?i gian th?c s? d?ng Photon Unity Networking (PUN2). M?i ng??i ch?i s? l?n l??t th?c hi?n c�c b??c di chuy?n tr�n b�n c? Match-3, t?o ra damage ?? t?n c�ng ??i th?.

## Ki?n tr�c h? th?ng

### 1. NetworkManager
- Qu?n l� k?t n?i Photon
- T?o v� tham gia ph�ng
- X? l� s? ki?n k?t n?i/ng?t k?t n?i
- ??ng b? scene gi?a c�c client

### 2. PvPGameManager
- Qu?n l� logic game PvP
- X? l� l??t ch?i
- T�nh to�n damage v� health
- ??ng b? game state

### 3. PvPPlayer
- Qu?n l� th�ng tin ng??i ch?i
- Health, mana, character stats
- T�nh to�n damage d?a tr�n tile matches

### 4. NetworkGameManager
- T�ch h?p v?i BoardManager
- ??ng b? board state
- X? l� tile swaps qua network

### 5. UI Components
- PvPPanel: Lobby UI
- PvPGamePanel: In-game UI
- PvPManager: Coordinator ch�nh

## C�ch thi?t l?p

### 1. C�i ??t Photon
1. Import PUN2 package t? Unity Asset Store
2. T?o t�i kho?n Photon t?i https://dashboard.photonengine.com
3. L?y App ID v� nh?p v�o Window > Photon Unity Networking > PUN Wizard

### 2. Scene Setup
1. T?o scene "PvPLobby" cho lobby
2. T?o scene "PvPGame" cho gameplay
3. Th�m c�c scenes v�o Build Settings

### 3. Prefab Setup
1. T?o NetworkManager prefab v?i NetworkManager script
2. T?o PvPManager prefab v?i PvPManager script
3. T?o UI prefabs cho PvPPanel v� PvPGamePanel

### 4. PhotonView Setup
C�c objects c?n PhotonView:
- NetworkManager
- PvPGameManager
- PvPPlayer instances
- NetworkGameManager

## Quy tr�nh ch?i

### 1. Lobby Phase
1. Ng??i ch?i k?t n?i t?i Photon
2. T?o ph�ng ho?c tham gia ph�ng c� s?n
3. Ch?n character
4. Ch? ??i th? s?n s�ng
5. B?t ??u game

### 2. Game Phase
1. Kh?i t?o board v� player stats
2. B?t ??u l??t ??u ti�n
3. Ng??i ch?i hi?n t?i th?c hi?n di chuy?n
4. T�nh to�n matches v� damage
5. Chuy?n l??t
6. L?p l?i cho ??n khi c� ng??i th?ng

### 3. End Game
1. Hi?n th? k?t qu?
2. T�y ch?n ch?i l?i ho?c v? lobby

## Network Messages

### TileSwapMessage
```csharp
{
    "tile1Position": Vector2Int,
    "tile2Position": Vector2Int,
    "playerId": int,
    "timestamp": float
}
```

### TileMatchMessage
```csharp
{
    "matchedTiles": List<Vector2Int>,
    "tileType": TileType,
    "damage": int,
    "playerId": int,
    "timestamp": float
}
```

### GameStateMessage
```csharp
{
    "gameState": GameState,
    "currentPlayerId": int,
    "turnTimeLeft": float,
    "turnNumber": int,
    "timestamp": float
}
```

## T�nh to�n Damage

### Base Damage
- M?i match-3: 10 damage
- M?i tile th�m: +5 damage
- Bonus theo lo?i tile: +2-6 damage

### Character Bonuses
- M?i character c� affinity v?i 1 lo?i tile
- Affinity bonus: +30% damage
- Critical hit: 10% chance, 1.5x damage

### Type Multipliers
- Pyro: +5 damage
- Hydro: +3 damage
- Electro: +4 damage
- Cryo: +3 damage
- Dendro: +4 damage
- Geo: +6 damage
- Anemo: +2 damage

## ??ng b? h�a

### Board State
- Master Client ??ng b? board state
- Tile swaps ???c g?i qua RPC
- Auto-sync m?i 0.1s

### Player State
- Health, mana ???c sync real-time
- Character selection ???c broadcast
- Ready state ???c sync

### Game State
- Turn management
- Timer synchronization
- Win/lose conditions

## X? l� l?i

### Network Disconnection
- Detect player disconnect
- Award win to remaining player
- Return to lobby

### Desync Issues
- Periodic state synchronization
- Conflict resolution (Master Client authority)
- Rollback mechanisms

### Input Validation
- Server-side validation
- Anti-cheat measures
- Rate limiting

## Testing

### Local Testing
1. Build game v� ch?y 2 instances
2. Ho?c d�ng Unity Editor + Build
3. Test c�c scenarios: connect, disconnect, gameplay

### Network Testing
1. Test v?i network latency
2. Test disconnect scenarios
3. Test multiple rooms

## Performance Optimization

### Network Optimization
- Compress message data
- Batch multiple updates
- Use delta compression
- Reduce sync frequency khi kh�ng c?n thi?t

### Memory Management
- Pool network messages
- Cleanup disconnected players
- Optimize PhotonView usage

## Troubleshooting

### Common Issues
1. **Cannot connect**: Check App ID v� network
2. **Desync**: Verify PhotonView components
3. **Performance**: Optimize network messages
4. **UI not updating**: Check event subscriptions

### Debug Tools
- PhotonNetwork.IsConnected
- PhotonNetwork.NetworkingClient.State
- PhotonNetwork.CurrentRoom.PlayerCount
- Debug.Log network messages

## M? r?ng

### Features c� th? th�m
- Ranked matchmaking
- Spectator mode
- Replay system
- Chat system
- Tournament mode
- Custom game modes

### Scalability
- Room management
- Server regions
- Load balancing
- Database integration