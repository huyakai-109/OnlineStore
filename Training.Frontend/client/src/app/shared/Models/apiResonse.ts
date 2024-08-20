import { Product } from './product';
import { Pagination } from './pagination';

export type ApiResponse = {
    pagination: Pagination;
    result: Product[];
    success: boolean;
    error: string | null;
};
