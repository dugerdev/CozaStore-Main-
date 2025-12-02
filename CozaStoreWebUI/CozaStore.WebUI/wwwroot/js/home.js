// Home sayfası için JavaScript - Wishlist işlemleri ve Search filtreleme

document.addEventListener('DOMContentLoaded', function() {
    // Search input için filtreleme
    const searchInput = document.getElementById('home-search-input');
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase().trim();
            filterProductsBySearch(searchTerm);
        });

        // Enter tuşuna basıldığında Shop sayfasına yönlendir
        searchInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                const searchTerm = this.value.trim();
                if (searchTerm) {
                    window.location.href = `/Shop?search=${encodeURIComponent(searchTerm)}`;
                }
            }
        });
    }
    // Tüm wishlist butonlarını bul
    const wishlistButtons = document.querySelectorAll('.js-addwish-b2');
    
    wishlistButtons.forEach(function(button) {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Product ID'yi data attribute'dan al
            const productId = this.getAttribute('data-product-id');
            if (!productId) {
                alert('Ürün ID bulunamadı.');
                return;
            }
            
            // Anti-forgery token al
            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            let token = '';
            
            if (tokenInput) {
                token = tokenInput.value;
            } else {
                // Alternatif olarak sayfadaki herhangi bir form'dan token al
                const form = document.querySelector('form');
                if (form) {
                    const formToken = form.querySelector('input[name="__RequestVerificationToken"]');
                    if (formToken) {
                        token = formToken.value;
                    }
                }
            }
            
            if (!token) {
                // Token yoksa meta tag'den al
                const metaToken = document.querySelector('meta[name="__RequestVerificationToken"]');
                if (metaToken) {
                    token = metaToken.getAttribute('content');
                }
            }
            
            if (!token) {
                alert('Güvenlik token bulunamadı. Lütfen sayfayı yenileyin.');
                return;
            }
            
            addToWishlist(productId, token, this);
        });
    });
});

// Wishlist'e ekleme fonksiyonu
function addToWishlist(productId, token, buttonElement) {
    // AJAX ile wishlist'e ekle
    fetch('/Wishlist/Add', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: `productId=${productId}&__RequestVerificationToken=${token}`
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('HTTP error! status: ' + response.status);
        }
        return response.json();
    })
    .then(data => {
        if (data.success) {
            // Başarılı mesajı göster
            alert('Ürün wishlist\'e eklendi!');
            
            // Header'daki wishlist sayısını güncelle
            if (typeof window.updateWishlistIcon === 'function') {
                window.updateWishlistIcon('wishlist-icon', data.count || 0);
                window.updateWishlistIcon('wishlist-icon-mobile', data.count || 0);
            } else {
                // Fallback
                const wishlistIcons = document.querySelectorAll('.icon-header-noti[href="/Wishlist/Index"]');
                wishlistIcons.forEach(icon => {
                    icon.setAttribute('data-notify', data.count || 0);
                });
            }
            
            // Wishlist sayısını güncelle (async olarak)
            if (typeof window.updateWishlistCount === 'function') {
                setTimeout(() => {
                    window.updateWishlistCount();
                }, 100);
            }
            
            // Buton görünümünü güncelle (isteğe bağlı)
            if (buttonElement) {
                buttonElement.classList.add('active');
            }
        } else {
            alert('Hata: ' + (data.error || 'Ürün wishlist\'e eklenemedi.'));
        }
    })
    .catch(error => {
        console.error('Error:', error);
        // Sadece gerçek bir hata varsa göster
        if (error.message && !error.message.includes('HTTP error')) {
            alert('Bir hata oluştu. Lütfen tekrar deneyin.');
        }
    });
}

// Ürünleri arama terimine göre filtrele (Isotope kullanarak)
function filterProductsBySearch(searchTerm) {
    if (typeof $ !== 'undefined' && $.fn.isotope) {
        const $grid = $('.isotope-grid');
        
        if ($grid.length > 0) {
            if (searchTerm === '') {
                // Tüm ürünleri göster
                $grid.isotope({ filter: '*' });
            } else {
                // Arama terimine göre filtrele
                $grid.isotope({ 
                    filter: function() {
                        const $item = $(this);
                        const productName = $item.find('.js-name-b2').text().toLowerCase();
                        const productPrice = $item.find('.stext-105').text().toLowerCase();
                        
                        return productName.includes(searchTerm) || productPrice.includes(searchTerm);
                    }
                });
            }
        }
    } else {
        // jQuery/Isotope yüklenmemişse, basit CSS filtreleme
        const items = document.querySelectorAll('.isotope-item');
        items.forEach(function(item) {
            const productName = item.querySelector('.js-name-b2')?.textContent?.toLowerCase() || '';
            const productPrice = item.querySelector('.stext-105')?.textContent?.toLowerCase() || '';
            
            if (searchTerm === '' || productName.includes(searchTerm) || productPrice.includes(searchTerm)) {
                item.style.display = '';
            } else {
                item.style.display = 'none';
            }
        });
    }
}

