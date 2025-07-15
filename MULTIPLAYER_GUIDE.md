# H??ng d?n PvP Mode v?i Photon

## T?ng quan

PvP Mode cho phép 2 ng??i ch?i ??u v?i nhau trong th?i gian th?c s? d?ng Photon Unity Networking (PUN2). M?i ng??i ch?i s? l?n l??t th?c hi?n các b??c di chuy?n trên bàn c? Match-3, t?o ra damage ?? t?n công ??i th?.

## Ki?n trúc h? th?ng

### 1. NetworkManager
- Qu?n lý k?t n?i Photon
- T?o và tham gia phòng
- X? lý s? ki?n k?t n?i/ng?t k?t n?i
- ??ng b? scene gi?a các client

### 2. PvPGameManager
- Qu?n lý logic game PvP
- X? lý l??t ch?i
- Tính toán damage và health
- ??ng b? game state

### 3. PvPPlayer
- Qu?n lý thông tin ng??i ch?i
- Health, mana, character stats
- Tính toán damage d?a trên tile matches

### 4. NetworkGameManager
- Tích h?p v?i BoardManager
- ??ng b? board state
- X? lý tile swaps qua network

### 5. UI Components
- PvPPanel: Lobby UI
- PvPGamePanel: In-game UI
- PvPManager: Coordinator chính

## Cách thi?t l?p

### 1. Cài ??t Photon
1. Import PUN2 package t? Unity Asset Store
2. T?o tài kho?n Photon t?i https://dashboard.photonengine.com
3. L?y App ID và nh?p vào Window > Photon Unity Networking > PUN Wizard

### 2. Scene Setup
1. T?o scene "PvPLobby" cho lobby
2. T?o scene "PvPGame" cho gameplay
3. Thêm các scenes vào Build Settings

### 3. Prefab Setup
1. T?o NetworkManager prefab v?i NetworkManager script
2. T?o PvPManager prefab v?i PvPManager script
3. T?o UI prefabs cho PvPPanel và PvPGamePanel

### 4. PhotonView Setup
Các objects c?n PhotonView:
- NetworkManager
- PvPGameManager
- PvPPlayer instances
- NetworkGameManager

## Quy trình ch?i

### 1. Lobby Phase
1. Ng??i ch?i k?t n?i t?i Photon
2. T?o phòng ho?c tham gia phòng có s?n
3. Ch?n character
4. Ch? ??i th? s?n sàng
5. B?t ??u game

### 2. Game Phase
1. Kh?i t?o board và player stats
2. B?t ??u l??t ??u tiên
3. Ng??i ch?i hi?n t?i th?c hi?n di chuy?n
4. Tính toán matches và damage
5. Chuy?n l??t
6. L?p l?i cho ??n khi có ng??i th?ng

### 3. End Game
1. Hi?n th? k?t qu?
2. Tùy ch?n ch?i l?i ho?c v? lobby

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

## Tính toán Damage

### Base Damage
- M?i match-3: 10 damage
- M?i tile thêm: +5 damage
- Bonus theo lo?i tile: +2-6 damage

### Character Bonuses
- M?i character có affinity v?i 1 lo?i tile
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

## ??ng b? hóa

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

## X? lý l?i

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
1. Build game và ch?y 2 instances
2. Ho?c dùng Unity Editor + Build
3. Test các scenarios: connect, disconnect, gameplay

### Network Testing
1. Test v?i network latency
2. Test disconnect scenarios
3. Test multiple rooms

## Performance Optimization

### Network Optimization
- Compress message data
- Batch multiple updates
- Use delta compression
- Reduce sync frequency khi không c?n thi?t

### Memory Management
- Pool network messages
- Cleanup disconnected players
- Optimize PhotonView usage

## Troubleshooting

### Common Issues
1. **Cannot connect**: Check App ID và network
2. **Desync**: Verify PhotonView components
3. **Performance**: Optimize network messages
4. **UI not updating**: Check event subscriptions

### Debug Tools
- PhotonNetwork.IsConnected
- PhotonNetwork.NetworkingClient.State
- PhotonNetwork.CurrentRoom.PlayerCount
- Debug.Log network messages

## M? r?ng

### Features có th? thêm
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