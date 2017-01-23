import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'dashed'
})

export class DashedPipe implements PipeTransform {
  transform(val: number, length = 6) {
    let dashes = '';
    let i = 0;
    while (i < length) {
      dashes = `${dashes}—`;
      i++;
    }
    const callNumMasked = `${val}${dashes}`;
    return callNumMasked.slice(0, length);
  }
}
