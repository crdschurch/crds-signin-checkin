export class MachineConfiguration {
  // this should probably live on a cookie service
  public static COOKIE_NAME = 'machine_config';

  private KIOSK_TYPE = {
    SIGN_IN: 1
  };

  KioskConfigId: number;
  KioskIdentifier: string;
  KioskName: string;
  KioskDescription: string;
  KioskTypeId: number;
  LocationId: number;
  CongregationId: number;
  RoomId: number;
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
    machineConfig.RoomId = json.RoomId;
    machineConfig.StartDate = json.StartDate;
    machineConfig.EndDate = json.EndDate;
    return machineConfig;
  }

  public isTypeSignIn(): boolean {
    return this.KioskTypeId === this.KIOSK_TYPE.SIGN_IN;
  }
}
