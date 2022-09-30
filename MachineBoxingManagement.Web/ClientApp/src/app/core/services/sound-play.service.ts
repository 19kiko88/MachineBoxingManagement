import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.vt02';
import { Enum_Sound } from '../../shared/models/enum/sound';

@Injectable({
  providedIn: 'root'
})
export class SoundPlayService {

  constructor() { }

  async playSound(enumSound: Enum_Sound)
  {
    let soundName = "";

    switch (enumSound) {
      case 0://error
        soundName = "error.mp3";
        break;
      case 1://alarm
        soundName = "alarm.mp3";
        break;
    }

    let audio = new Audio();
    audio.src = `${environment.webSite}/assets/${soundName}`
    audio.load();
    audio.play();
  }
}
