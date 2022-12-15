import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/internal/operators/tap';
import { IResultDto } from '../../shared/models/dto/result-dto';
import { BaseService } from './base.service';

@Injectable({
  providedIn: 'root'
})

export class JwtService extends BaseService {

  constructor(
    private httpClient: HttpClient
  ) {
    super(); 
  }

  //取得JWT
  getJwt(): Observable<IResultDto<string>> {
    const url = `/Jwt/GetJWT`;
    const options = this.generatePostOptions();

    return this.httpClient.get<IResultDto<string>>(url, options)
      .pipe(
        tap(() => {
          console.log("execute api GetJWT.");
        })
      )
  }

  //JWT(是否過期)檢核
  jwtValidate(jwt: string): Observable<IResultDto<boolean>> {
    const url = `/Jwt/JwtValidate?jwt=${jwt}`;
    const options = this.generatePostOptions();

    return this.httpClient.get<IResultDto<boolean>>(url, options)
      .pipe(
        tap(() => {
          console.log("execute api jwtValidate.");
        })
      )
  }
}
