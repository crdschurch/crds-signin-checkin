export class MachineConfiguration {
  // this should probably live on a cookie service
  public static COOKIE_NAME_ID = 'machine_config_id';
  public static COOKIE_NAME_DETAILS = 'machine_config_details';

  private KIOSK_TYPE = {
    SIGN_IN: 1,
    ROOM_CHECKIN: 2,
    ADMIN: 3
  };

  KioskConfigId: number;
  KioskIdentifier: string;
  KioskName: string;
  KioskDescription: string;
  KioskTypeId: number;
  LocationId: number;
  CongregationId: number;
  CongregationName: string;
  RoomId: number;
  RoomName: string;
  StartDate: string;
  EndDate: string;

  static fromJson(json: any): MachineConfiguration {
    if (!json) {
      return new MachineConfiguration();
    }

    let machineConfig = new MachineConfiguration();
    machineConfig.KioskConfigId = json.KioskConfigId;
    machineConfig.KioskIdentifier = json.KioskIdentifier;
    machineConfig.KioskName = json.KioskName;
    machineConfig.KioskDescription = json.KioskDescription;
    machineConfig.KioskTypeId = json.KioskTypeId;
    machineConfig.LocationId = json.LocationId;
    machineConfig.CongregationId = json.CongregationId;
    machineConfig.CongregationName = json.CongregationName;
    machineConfig.RoomId = json.RoomId;
    machineConfig.RoomName = json.RoomName;
    machineConfig.StartDate = json.StartDate;
    machineConfig.EndDate = json.EndDate;
    return machineConfig;
  }

  public isTypeSignIn(): boolean {
    return this.KioskTypeId === this.KIOSK_TYPE.SIGN_IN;
  }

  public isTypeRoomCheckin(): boolean {
    return this.KioskTypeId === this.KIOSK_TYPE.ROOM_CHECKIN;
  }

  public isTypeAdmin(): boolean {
    return this.KioskTypeId === this.KIOSK_TYPE.ADMIN;
  }

  public kioskType(): string {
    if (this.isTypeSignIn()) {
      return 'Sign In';
    }

    return 'Check In';
  }
}
