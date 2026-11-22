
using System;
using System.Collections.Generic;

namespace TasinmazYonetimi.DTOs
{
    public class PagedResultDto<T>
    {
        // Sayfadaki kayıtlar
        public List<T> Items { get; set; } = new();

        // Filtreye uyan toplam kayıt sayısı
        public int TotalCount { get; set; }

        // Mevcut sayfa numarası
        public int PageNumber { get; set; }

        // Bir sayfada gösterilecek kayıt sayısı
        public int PageSize { get; set; }

        // Toplam sayfa sayısı
        public int TotalPages =>
            PageSize > 0
                ? (int)Math.Ceiling(
                    (double)TotalCount / PageSize)
                : 0;
    }
}
