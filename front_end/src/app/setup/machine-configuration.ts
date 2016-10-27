export class MachineConfiguration {
  Site: number;
  Type: number;
  Guid: string;

  static fromJson(json: any): MachineConfiguration {
    if (!json) {
      return new MachineConfiguration();
    }
    let machineConfiguration = new MachineConfiguration();
    console.log("hi", machineConfiguration, this);
    for (let prop in machineConfiguration) {
      console.log(prop)
      if (machineConfiguration.hasOwnProperty(prop)) {
        console.log("doing it")
        machineConfiguration[prop] = json[prop];
      }
    }
    return machineConfiguration;
  }
}
