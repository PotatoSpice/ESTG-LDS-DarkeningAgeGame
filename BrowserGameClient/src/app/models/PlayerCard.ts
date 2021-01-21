export class PlayerCard {

    playerId: string;
    faction: string;
    image: any;

    constructor(p_playerId: string, p_faction: string, p_image: any) {
        this.playerId = p_playerId;
        this.faction = p_faction;
        this.image = p_image;
    }
};