function calculate() {
            var amount = document.getElementById("vehicle-price");
            var interestRate = document.getElementById("interest-rate");
            var period = document.getElementById("period");
            var payment = document.getElementById("payment");

            var dawnPayment = document.getElementById("dawn-payment");

            var total = document.getElementById("total-payment");
            var totalinterest = document.getElementById("total-interest");

            var parsedDawnPayment = parseFloat(dawnPayment.value);

            var principal = parseFloat(amount.value) - parsedDawnPayment;
            var interest = parseFloat(interestRate.value) / 100 / 12;
            var payments = parseFloat(period.value); /**= 12*/
            var x = Math.pow(1 + interest, payments);
            var monthly = (principal * x * interest) / (x - 1);

            if (isFinite(monthly)) {
                payment.innerHTML = monthly.toFixed(2);
                total.innerHTML = (monthly * payments).toFixed(2);
                totalinterest.innerHTML = ((monthly * payments) - principal).toFixed(2);
                save(amount.value, interestRate.value, period.value, parsedDawnPayment);
                try {
                    getLenders(amount.value, interestRate.value, period.value);
                }
                catch (e) { /* And ignore those errors */ }
            }
            else {
                payment.innerHTML = "$";
                total.innerHTML = "$";
                totalinterest.innerHTML = "$";
            }
        }
        function save(amount, apr, years, dawnPayment) {
            if (window.localStorage) {
                localStorage.loan_amount = amount;
                localStorage.loan_apr = apr;
                localStorage.loan = years;
                localStorage.loan_dawn_payment = dawnPayment;
            }
        }
        function getLenders(amount, apr, years) {
            if (!window.XMLHttpRequest)
                return;
            var ad = document.getElementById("lenders");
            if (!ad)
                return;
            var url = "getLenders.php" +
                "?amt=" +
                encodeURIComponent(amount) +
                "&apr=" +
                encodeURIComponent(apr) +
                "&yrs=" +
                encodeURIComponent(years);
            var req = new XMLHttpRequest();
            req.open("GET", url);
            req.send(null);
            req.onreadystatechange = function () {
                if (req.readyState == 4 && req.status == 200) {
                    var response = req.responseText;
                    var lenders = JSON.parse(response);
                    var list = "";
                    for (var i = 0; i < lenders.length; i++) {
                        list += "<li>< a href = '" + lenders[i].url + "' > " + lenders[i].name + "</a > ";
                    }
                    ad.innerHTML = "<ul>" + list + "</ul>";
                }
            };
        }