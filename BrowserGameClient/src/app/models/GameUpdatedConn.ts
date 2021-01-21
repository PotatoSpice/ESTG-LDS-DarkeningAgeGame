export class GamePlayerData {
    playerId: string;
    faction: string;

    constructor(p_playerId: string, p_faction: string) {
        this.playerId = p_playerId;
        this.faction = p_faction;
    }
};

export class GameUpdatedConn {
    data: GamePlayerData[];

    constructor(p_data: GamePlayerData[]) {
        this.data = p_data;
    }
};