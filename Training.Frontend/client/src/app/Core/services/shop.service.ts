import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ApiResponse } from '../../shared/Models/apiResonse';

@Injectable({
    providedIn: 'root',
})
export class ShopService {
    baseUrl = 'https://localhost:7051/api/';
    private http = inject(HttpClient);

    getProducts(sort?: string) {
        let params = new HttpParams();

        if (sort) {
            params = params.append('sort', sort);
        }
        return this.http.get<ApiResponse>(this.baseUrl + 'products', {
            params,
        });
    }
}
