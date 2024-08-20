import { Component, inject, OnInit } from '@angular/core';
import { Product } from '../../shared/Models/product';
import { Pagination } from '../../shared/Models/pagination';
import { ShopService } from '../../Core/services/shop.service';
import { MatCard, MatCardContent } from '@angular/material/card';
import { ProductItemComponent } from './product-item/product-item.component';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import {
    MatListOption,
    MatSelectionList,
    MatSelectionListChange,
} from '@angular/material/list';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';

@Component({
    selector: 'app-shop',
    standalone: true,
    imports: [
        ProductItemComponent,
        MatButton,
        MatIcon,
        MatMenu,
        MatSelectionList,
        MatListOption,
        MatMenuTrigger,
    ],
    templateUrl: './shop.component.html',
    styleUrl: './shop.component.scss',
})
export class ShopComponent implements OnInit {
    private shopService = inject(ShopService);
    products: Product[] = [];
    pagination!: Pagination;
    selectedSort: string = 'name';
    sortOption = [
        { name: 'Price: Low to High', value: 'asc' },
        { name: 'Price: High to Low', value: 'desc' },
    ];

    ngOnInit(): void {
        this.initialiseShop();
    }
    initialiseShop() {
        this.getProduct();
    }
    getProduct() {
        this.shopService.getProducts(this.selectedSort).subscribe({
            next: (response) => {
                this.products = response.result;
                this.pagination = response.pagination;
            },
            error: (error) => console.log(error),
        });
    }
    onSortChange(event: MatSelectionListChange) {
        const selectedOption = event.options[0];
        if (selectedOption) {
            this.selectedSort = selectedOption.value;
            this.getProduct();
        }
    }
}
