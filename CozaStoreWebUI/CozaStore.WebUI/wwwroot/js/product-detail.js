// Product Detail sayfası için JavaScript

document.addEventListener('DOMContentLoaded', function() {
    const addToCartForm = document.getElementById('addToCartForm');
    
    if (addToCartForm) {
        addToCartForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            const productId = this.querySelector('input[name="productId"]').value;
            const quantity = parseInt(this.querySelector('input[name="quantity"]').value) || 1;
            
            // Anti-forgery token al
            const token = this.querySelector('input[name="__RequestVerificationToken"]').value;
            
            // AJAX ile sepete ekle
            fetch('/Cart/Add', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: `productId=${productId}&quantity=${quantity}&__RequestVerificationToken=${token}`
            })
            .then(response => {
                // Response başarılı mı kontrol et
                if (!response.ok) {
                    throw new Error('HTTP error! status: ' + response.status);
                }
                // Content-Type JSON mi kontrol et
                const contentType = response.headers.get("content-type");
                if (contentType && contentType.indexOf("application/json") !== -1) {
                    return response.json();
                } else {
                    throw new Error('Response is not JSON');
                }
            })
            .then(data => {
                if (data.success) {
                    // Başarılı mesajı göster
                    alert('Ürün sepete eklendi!');
                    
                    // Header'daki cart sayısını güncelle
                    if (typeof window.updateCartCount === 'function') {
                        window.updateCartCount();
                    } else {
                        // Fallback: Manuel güncelleme
                        updateCartIconFallback(data.count || 0);
                    }
                } else {
                    alert('Hata: ' + (data.error || 'Ürün sepete eklenemedi.'));
                }
            })
            .catch(error => {
                // Sadece gerçek hata durumlarında alert göster
                // JSON parse hatası veya network hatası olabilir
                if (!error.message.includes('JSON')) {
                    alert('Bir hata oluştu: ' + error.message);
                }
            });
        });
    }

    // Wishlist butonu için event handler
    const wishlistButton = document.querySelector('.js-addwish-detail');
    if (wishlistButton) {
        wishlistButton.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Product ID'yi al (form'dan veya data attribute'dan)
            const productIdInput = document.querySelector('input[name="productId"]');
            if (!productIdInput) {
                alert('Ürün ID bulunamadı.');
                return;
            }
            
            const productId = productIdInput.value;
            
            // Anti-forgery token al
            const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
            if (!tokenInput) {
                alert('Güvenlik token bulunamadı.');
                return;
            }
            
            const token = tokenInput.value;
            
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
                    if (typeof window.updateWishlistCount === 'function') {
                        window.updateWishlistCount();
                    } else {
                        // Fallback: Manuel güncelleme
                        updateWishlistIconFallback(data.count || 0);
                    }
                    
                    // Buton görünümünü güncelle (isteğe bağlı)
                    wishlistButton.classList.add('active');
                } else {
                    alert('Hata: ' + (data.error || 'Ürün wishlist\'e eklenemedi.'));
                }
            })
            .catch(error => {
                // Sadece gerçek bir hata varsa göster
                if (error.message && !error.message.includes('HTTP error')) {
                    alert('Bir hata oluştu. Lütfen tekrar deneyin.');
                }
            });
        });
    }
});

// Fallback: Cart icon'unu manuel güncelle
function updateCartIconFallback(count) {
    const cartIcons = document.querySelectorAll('#cart-icon, #cart-icon-mobile');
    cartIcons.forEach(icon => {
        icon.setAttribute('data-notify', count);
    });
}

// Fallback: Wishlist icon'unu manuel güncelle
function updateWishlistIconFallback(count) {
    const wishlistIcons = document.querySelectorAll('#wishlist-icon, #wishlist-icon-mobile');
    wishlistIcons.forEach(icon => {
        icon.setAttribute('data-notify', count);
    });
}

